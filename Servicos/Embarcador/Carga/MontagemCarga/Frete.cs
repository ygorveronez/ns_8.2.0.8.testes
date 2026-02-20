namespace Servicos.Embarcador.Carga.MontagemCarga
{
    public class Frete : ServicoBase
    {
        AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware TipoServicoMultisoftware;

        public Frete(string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware) : base()
        {
            TipoServicoMultisoftware = tipoServicoMultisoftware;
        }

        //public Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete RecalcularFreteTabelaFrete(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, bool atualizarInformacoesPagamentoPedido = true)
        //{
        //    Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
        //    Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

        //    Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retorno = CalcularFreteExtornandoComplementos(ref carregamento, cargas, unitOfWork, atualizarInformacoesPagamentoPedido);

        //    foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargas)
        //    {
        //        if (carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador)
        //        {
        //            if (retorno.situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.FreteValido)
        //            {
        //                carga.ValorFrete = 0;
        //                carga.ValorFreteAPagar = 0;
        //                carga.ValorICMS = 0;
        //                carga.ValorFreteOperador = 0;

        //                if (configuracao.NotificarAlteracaoCargaAoOperador)
        //                    serCarga.NotificarAlteracaoAoOperador(carga, $"Não foi possível calcular o frete da carga n° {carga.CodigoCargaEmbarcador}", unitOfWork);
        //            }
        //            else
        //            {
        //                //todo: rever isso, pois está fixo apenas quando não calcula o frete da filial emissora, danone.
        //                if (carga.EmpresaFilialEmissora != null)
        //                {
        //                    Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retornoFilialEmissora = Servicos.Embarcador.Carga.FreteFilialEmissora.CalcularFreteFilialEmissora(carga, false, false, true, unitOfWork, TipoServicoMultisoftware, configuracao);

        //                    if (retornoFilialEmissora != null)
        //                    {
        //                        retorno.DadosFreteFilialEmissora = retornoFilialEmissora;

        //                        if (configuracao.NotificarAlteracaoCargaAoOperador && (retornoFilialEmissora.situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.ProblemaCalcularFrete))
        //                            serCarga.NotificarAlteracaoAoOperador(carga, $"Não foi possível calcular o frete da carga n° {carga.CodigoCargaEmbarcador}", unitOfWork);
        //                    }
        //                }
        //                else if (carga.TipoOperacao != null && carga.TipoOperacao.EmiteCTeFilialEmissora && carga.Filial != null && carga.Filial.EmpresaEmissora != null)
        //                {
        //                    Dominio.Entidades.Embarcador.Cargas.Carga cargaRef = carga;
        //                    Servicos.Embarcador.Carga.FreteFilialEmissora.SetarValorFreteFilialTrechoAnterior(ref cargaRef, false, TipoServicoMultisoftware, unitOfWork, configuracao);
        //                    repCarga.Atualizar(cargaRef);
        //                }
        //            }

        //            carga.TipoFreteEscolhido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Tabela;

        //            repCarga.Atualizar(carga);


        //        }
        //    }

        //    return retorno;
        //}

        //private Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete CalcularFreteExtornandoComplementos(ref Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas, Repositorio.UnitOfWork unitOfWork, bool atualizarInformacoesPagamentoPedido = true)
        //{
        //    Repositorio.Embarcador.Cargas.CargaComponentesFrete repCargaComponentesFrete = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(unitOfWork);
        //    Repositorio.Embarcador.Cargas.CargaTabelaFreteComponenteFrete repCargaTabelaFreteComponenteFrete = new Repositorio.Embarcador.Cargas.CargaTabelaFreteComponenteFrete(unitOfWork);
        //    Servicos.Embarcador.Carga.ComplementoFrete serComplementoFrete = new ComplementoFrete(unitOfWork);



        //    foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargas)
        //    {
        //        List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> cargaComponentesFrete = repCargaComponentesFrete.BuscarPorCarga(carga.Codigo);
        //        foreach (Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete componente in cargaComponentesFrete)
        //        {
        //            if (componente.CargaComplementoFrete != null)
        //            {
        //                serComplementoFrete.ExtornarComplementoDeFrete(componente.CargaComplementoFrete, TipoServicoMultisoftware, unitOfWork);
        //            }

        //            if (componente.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCargaComponenteFrete.Ocorrencia) //para remover tem que cancelar a ocorrência
        //                repCargaComponentesFrete.Deletar(componente);
        //        }

        //        List<Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteComponenteFrete> cargaTabelaFreteComponentesFrete = repCargaTabelaFreteComponenteFrete.BuscarPorCarga(carga.Codigo);

        //        foreach (Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteComponenteFrete cargaTabelaFreteComponenteFrete in cargaTabelaFreteComponentesFrete)
        //            repCargaTabelaFreteComponenteFrete.Deletar(cargaTabelaFreteComponenteFrete);
        //    }

        //    Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retornoFrete = ProcessarFrete(ref carregamento, cargas, unitOfWork, false, true, atualizarInformacoesPagamentoPedido);

        //    //if (carga.EmpresaFilialEmissora != null)
        //    //{
        //    //    Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retornoFilialEmissora = Servicos.Embarcador.Carga.FreteFilialEmissora.CalcularFreteFilialEmissora(carga, carga.TabelaFrete, false, true, atualizarInformacoesPagamentoPedido, unitOfWork, TipoServicoMultisoftware);
        //    //    if (retornoFilialEmissora != null)
        //    //        retornoFrete.DadosFreteFilialEmissora = retornoFilialEmissora;
        //    //}

        //    return retornoFrete;
        //}

        //private Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete ProcessarFrete(ref Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas, Repositorio.UnitOfWork unitOfWork, bool apenasVerificar, bool adicionarComponentesCarga, bool atualizarInformacoesPagamentoPedido = true)
        //{
        //    Repositorio.Embarcador.Cargas.CargaTabelaFreteSubContratacao repCargaTabelaFreteSubContratacao = new Repositorio.Embarcador.Cargas.CargaTabelaFreteSubContratacao(unitOfWork);

        //    Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFreteExistente = carregamento.TabelaFrete;
        //    Servicos.Embarcador.Carga.Frete serCargaFrete = new Embarcador.Carga.Frete(StringConexao, TipoServicoMultisoftware);
        //    Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

        //    Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
        //    Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retorno = new Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete() { };

        //    Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteSubContratacao cargaTabelaFreteSubContratacao = null;
        //    Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();

        //    foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargas)
        //    {

        //        if (carga.TipoFreteEscolhido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador)
        //        {
        //            retorno = new RetornoDadosFrete();
        //            retorno.situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.FreteValido;
        //            return retorno;
        //        }

        //        if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
        //        {
        //            if (carga.Pedidos != null && carga.Pedidos.All(obj => obj.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada))
        //                cargaTabelaFreteSubContratacao = repCargaTabelaFreteSubContratacao.BuscarPorCarga(carga.Codigo);
        //        }

        //        if (!apenasVerificar && (carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador || !carga.CalculandoFrete))
        //            Servicos.Embarcador.Carga.ComposicaoFrete.ComposicaoFrete.ExcluirComposicoesFrete(carga, unitOfWork);

        //    }

        //    StringBuilder mensagemRetorno = new StringBuilder();
        //    List<TabelaFrete> tabelasFrete = new List<TabelaFrete>();

        //    if (!apenasVerificar)
        //        tabelasFrete = ObterTabelasFrete(carregamento, unitOfWork, cargas, TipoServicoMultisoftware, ref mensagemRetorno);
        //    else if (tabelaFreteExistente != null)
        //        tabelasFrete.Add(tabelaFreteExistente);

        //    if (!ValidarQuantidadeTabelaFreteDisponivel(ref retorno, ref carregamento, cargas, tabelasFrete, mensagemRetorno, apenasVerificar, unitOfWork))
        //        return retorno;

        //    switch (tabelasFrete[0].TipoTabelaFrete)
        //    {
        //        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFrete.tabelaCliente:
        //            if (tabelasFrete[0].TipoCalculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoTabelaFrete.PorCarga)
        //            {
        //                if (CalcularFretePorCliente(ref retorno, ref carregamento, cargas, tabelasFrete[0], configuracao, apenasVerificar, unitOfWork, atualizarInformacoesPagamentoPedido, adicionarComponentesCarga, false))
        //                    return retorno;
        //            }
        //            else if (tabelasFrete[0].TipoCalculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoTabelaFrete.PorPedido)
        //            {
        //                retorno.situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.ProblemaCalcularFrete;
        //                retorno.mensagem = "Não é permitido agrupar cargas  com cálculo de frete por pedido";
        //                return retorno;

        //                //if (CalcularFretePorClientePedido(ref retorno, ref carga, tabelasFrete[0], apenasVerificar, unitOfWork, atualizarInformacoesPagamentoPedido, adicionarComponentesCarga, false))
        //                //    return retorno;
        //            }
        //            else if (tabelasFrete[0].TipoCalculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoTabelaFrete.PorDocumentoEmitido)
        //            {
        //                retorno.situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.ProblemaCalcularFrete;
        //                retorno.mensagem = "Não é permitido agrupar cargas com cálculo de frete por documento";
        //                return retorno;
        //            }

        //            break;
        //        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFrete.tabelaRota:
        //            retorno.situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.ProblemaCalcularFrete;
        //            retorno.mensagem = "Não é permitido agrupar cargas com cálculo de frete por Rota";
        //            return retorno;
        //        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFrete.tabelaComissaoProduto:
        //            retorno.situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.ProblemaCalcularFrete;
        //            retorno.mensagem = "Não é permitido agrupar cargas com cálculo de frete por Comissão de produto";
        //            return retorno;
        //    }

        //    if (!apenasVerificar)
        //    {
        //        foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargas)
        //        {
        //            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);
        //            serCargaFrete.ValidarValoresFrete(carga, cargaPedidos, unitOfWork);
        //            Seguro.Seguro.SetarDadosSeguroCarga(carga, cargaPedidos, configuracao, TipoServicoMultisoftware, unitOfWork);
        //        }
        //    }

        //    foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargas)
        //        serCargaFrete.SetarDadosGeraisRetornoFrete(ref retorno, carga, unitOfWork, apenasVerificar, false, carga.TipoFreteEscolhido);

        //    return retorno;
        //}

        //private bool CalcularFretePorCliente(ref Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retorno, ref Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas, Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, bool apenasVerificar, Repositorio.UnitOfWork unitOfWork, bool atualizarInformacoesPagamentoPedido, bool adicionarComponentesCarga, bool calculoFreteFilialEmissora)
        //{
        //    Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

        //    Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
        //    Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
        //    Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
        //    Repositorio.Embarcador.Cargas.CargaTabelaFreteCliente repCargaTabelaFreteCliente = new Repositorio.Embarcador.Cargas.CargaTabelaFreteCliente(unitOfWork);

        //    Servicos.Embarcador.Carga.FreteCTeSubcontratacao serFreteCTeSubcontratacao = new FreteCTeSubcontratacao(unitOfWork);
        //    Servicos.Embarcador.Carga.FreteCliente svcFreteCliente = new FreteCliente(StringConexao);
        //    Servicos.Embarcador.Carga.Frete serCargaFrete = new Embarcador.Carga.Frete(StringConexao, TipoServicoMultisoftware);
        //    StringBuilder mensagemRetorno = new StringBuilder();

        //    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarregamento(carregamento.Codigo);

        //    Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametrosCalculo = ObterParametrosCalculoFretePorCarga(tabelaFrete, carregamento, cargas, cargaPedidos, calculoFreteFilialEmissora, unitOfWork, StringConexao, TipoServicoMultisoftware);

        //    List<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente> tabelasCliente = svcFreteCliente.ObterTabelasFrete(ref mensagemRetorno, parametrosCalculo, tabelaFrete, unitOfWork, TipoServicoMultisoftware);

        //    if ((tabelasCliente.Count <= 0 || tabelasCliente.Count > 1))
        //    {
        //        var retornar = false;
        //        foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargas)
        //        {

        //            if (carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador)
        //            {
        //                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

        //                retorno = new Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete();
        //                retorno.tipoTabelaFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFrete.tabelaCliente;
        //                retorno.situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.ProblemaCalcularFrete;

        //                foreach (var item in tabelasCliente)
        //                {
        //                    Servicos.Log.TratarErro(item.Descricao);
        //                }
        //                retorno.mensagem = tabelasCliente.Count <= 0 ? "Não foi localizada uma configuração de frete para a tabela de frete " + tabelaFrete.Descricao + " compatível com as configurações da carga.\n" : "Foi encontrada mais configuração de frete disponível para a carga para a tabela de frete " + tabelaFrete.Descricao + ".";

        //                if (!apenasVerificar)
        //                {
        //                    carga.MotivoPendenciaFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoPendenciaFrete.ProblemaCalculoFrete;

        //                    if (serCarga.VerificarSeCargaEstaNaLogistica(carga, TipoServicoMultisoftware))
        //                    {
        //                        carga.PossuiPendencia = true;
        //                        carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete;
        //                    }

        //                    carga.MotivoPendencia = retorno.mensagem.Length < 2000 ? retorno.mensagem : retorno.mensagem.Substring(0, 1999);

        //                    if (!calculoFreteFilialEmissora)
        //                        carga.TabelaFrete = null;
        //                    else
        //                        carga.TabelaFreteFilialEmissora = null;

        //                    repCarga.Atualizar(carga);

        //                    Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteCliente cargaTabelaCliente = repCargaTabelaFreteCliente.BuscarPorCarga(carga.Codigo, calculoFreteFilialEmissora);

        //                    if (cargaTabelaCliente != null)
        //                        repCargaTabelaFreteCliente.Deletar(cargaTabelaCliente);

        //                    if (!calculoFreteFilialEmissora)
        //                    {
        //                        Seguro.Seguro.SetarDadosSeguroCarga(carga, cargaPedidos, configuracao, TipoServicoMultisoftware, unitOfWork);
        //                    }
        //                }

        //                if (carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador)
        //                    retornar = true;
        //            }
        //        }

        //        mensagemRetorno.Append(retorno.mensagem);
        //        if (retornar)
        //            return true;

        //        if (!calculoFreteFilialEmissora)
        //        {
        //            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
        //                serCargaFrete.AjustarCargaPedidoTabelaNaoExiste(cargaPedido, unitOfWork);

        //        }
        //    }
        //    else
        //    {
        //        retorno = svcFreteCliente.SetarTabelaFreteCarregamento(ref carregamento, parametrosCalculo, cargas, tabelasCliente[0], configuracao, unitOfWork, apenasVerificar, TipoServicoMultisoftware, atualizarInformacoesPagamentoPedido, adicionarComponentesCarga, calculoFreteFilialEmissora);
        //    }

        //    retorno.situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.FreteValido;

        //    return false;
        //}

        //private bool ValidarQuantidadeTabelaFreteDisponivel(ref Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retorno, ref Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas, List<Dominio.Entidades.Embarcador.Frete.TabelaFrete> tabelasFrete, StringBuilder mensagemRetorno, bool apenasVerificar, Repositorio.UnitOfWork unitOfWork)
        //{
        //    bool retornar = true;
        //    if (tabelasFrete.Count <= 0 || tabelasFrete.Count > 1)
        //    {
        //        Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
        //        Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
        //        Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
        //        Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

        //        retorno = new Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete();

        //        foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargas)
        //        {
        //            if (carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Embarcador)
        //            {
        //                retorno.tipoTabelaFrete = tabelasFrete.Count > 0 ? tabelasFrete.Select(o => o.TipoTabelaFrete).First() : Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFrete.tabelaCliente;
        //                retorno.situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.ProblemaCalcularFrete;
        //                //retorno.dadosRetornoTipoFrete = new Dominio.ObjetosDeValor.Embarcador.Frete.FreteCliente() { situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoFreteCliente.TabelaFreteNaoCadastrada };

        //                string tabelas = "";
        //                if (tabelasFrete.Count > 0)
        //                    tabelas = string.Join(", ", from obj in tabelasFrete select obj.Descricao);

        //                retorno.mensagem = tabelasFrete.Count <= 0 ? mensagemRetorno.Insert(0, "Não foi localizada uma tabela de frete compatível com as configurações da carga.\n").ToString() : "Foi encontrada mais de uma tabela de frete disponível para a carga (" + tabelas + ").";

        //                if (!apenasVerificar)
        //                {
        //                    carga.MotivoPendenciaFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoPendenciaFrete.ProblemaCalculoFrete;
        //                    Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
        //                    if (serCarga.VerificarSeCargaEstaNaLogistica(carga, TipoServicoMultisoftware))
        //                    {
        //                        carga.PossuiPendencia = true;
        //                        carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete;
        //                    }
        //                    carga.MotivoPendencia = retorno.mensagem.Length < 2000 ? retorno.mensagem : retorno.mensagem.Substring(0, 1999);

        //                    repCarga.Atualizar(carga);

        //                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);
        //                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
        //                    {
        //                        cargaPedido.IncluirICMSBaseCalculo = true;

        //                        if (!cargaPedido.Pedido.AdicionadaManualmente)
        //                        {
        //                            if (cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada)
        //                                cargaPedido.Pedido.UsarTipoPagamentoNF = true;
        //                            else
        //                                cargaPedido.Pedido.UsarTipoPagamentoNF = false;

        //                            if (repPedidoXMLNotaFiscal.ContarPorCargaPedido(cargaPedido.Codigo) > 0)
        //                            {
        //                                Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete? modalidadePagamentoFrete = repPedidoXMLNotaFiscal.BuscarModalidadeDeFretePadraoPorCargaPedido(cargaPedido.Codigo);
        //                                if (modalidadePagamentoFrete.HasValue && modalidadePagamentoFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.NaoDefinido)
        //                                    cargaPedido.Pedido.TipoPagamento = (Dominio.Enumeradores.TipoPagamento)modalidadePagamentoFrete;
        //                                else
        //                                {
        //                                    cargaPedido.Pedido.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Pago;
        //                                }
        //                            }
        //                            else
        //                            {
        //                                cargaPedido.Pedido.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Pago;
        //                            }

        //                            if (cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.SubContratada && cargaPedido.RegraTomador == null)
        //                            {
        //                                if (cargaPedido.Pedido.TipoPagamento == Dominio.Enumeradores.TipoPagamento.A_Pagar)
        //                                    cargaPedido.TipoTomador = Dominio.Enumeradores.TipoTomador.Destinatario;
        //                                else
        //                                    cargaPedido.TipoTomador = Dominio.Enumeradores.TipoTomador.Remetente;
        //                            }

        //                            repCargaPedido.Atualizar(cargaPedido);
        //                            repPedido.Atualizar(cargaPedido.Pedido);
        //                        }
        //                    }

        //                    //Seguro.Seguro.SetarDadosSeguroCarga(carga, cargaPedidos, TipoServicoMultisoftware, unitOfWork);
        //                }
        //            }
        //            else
        //            {
        //                retorno.situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.FreteValido;
        //            }
        //        }
        //        retornar = false;
        //    }

        //    return retornar;
        //}

        //private List<Dominio.Entidades.Embarcador.Frete.TabelaFrete> ObterTabelasFrete(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, Repositorio.UnitOfWork unidadeDeTrabalho, List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, ref StringBuilder mensagem, Dominio.Entidades.Cliente tomador = null, bool pagamentoTerceiro = false, bool tabelaFreteMinima = false)
        //{
        //    List<Dominio.Entidades.Embarcador.Frete.TabelaFrete> tabelasFrete = new List<Dominio.Entidades.Embarcador.Frete.TabelaFrete>();
        //    Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unidadeDeTrabalho);
        //    Repositorio.Embarcador.Frete.TabelaFrete repTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFrete(unidadeDeTrabalho);
        //    Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeDeTrabalho);

        //    Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
        //    Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete = null;


        //    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoPadrao = repCargaPedido.BuscarPrimeiraPorCarga(cargas.FirstOrDefault().Codigo);

        //    DateTime dataParaVerificacaoVigencia = carregamento.DataCarregamentoCarga;
        //    if (configuracao.ValidarTabelaFreteComDataAtual)
        //        dataParaVerificacaoVigencia = DateTime.Now;

        //    if (tomador == null)
        //        tomador = cargaPedidoPadrao.ObterTomador();

        //    List<int> filiais = (from obj in cargas select obj.Filial.Codigo).Distinct().ToList();
        //    List<int> tiposOperacao = (from obj in cargas select obj.TipoOperacao.Codigo).Distinct().ToList();

        //    if (carregamento.Empresa != null)
        //    {
        //        tabelasFrete = repTabelaFrete.BuscarPorEmpresa(carregamento.Empresa.Codigo, pagamentoTerceiro, tabelaFreteMinima);
        //        if (tabelasFrete.Count > 0)
        //            tabelaFrete = RetornarTabelaValidaValida(tabelasFrete, tomador.GrupoPessoas?.Codigo ?? 0, filiais, tiposOperacao, carregamento.Empresa?.Codigo ?? 0, dataParaVerificacaoVigencia);
        //    }

        //    if (tabelaFrete == null && filiais.Count > 0)
        //    {
        //        tabelasFrete = repTabelaFrete.BuscarPorFilial(filiais, pagamentoTerceiro, tabelaFreteMinima);
        //        if (tabelasFrete.Count > 0)
        //            tabelaFrete = RetornarTabelaValidaValida(tabelasFrete, tomador.GrupoPessoas?.Codigo ?? 0, filiais, tiposOperacao, carregamento.Empresa?.Codigo ?? 0, dataParaVerificacaoVigencia);

        //    }
        //    if (tabelaFrete == null && tiposOperacao.Count > 0)
        //    {
        //        tabelasFrete = repTabelaFrete.BuscarPorTipoOperacao(tiposOperacao, pagamentoTerceiro, tabelaFreteMinima);
        //        if (tabelasFrete.Count > 0)
        //            tabelaFrete = RetornarTabelaValidaValida(tabelasFrete, tomador.GrupoPessoas?.Codigo ?? 0, filiais, tiposOperacao, carregamento.Empresa?.Codigo ?? 0, dataParaVerificacaoVigencia);
        //    }

        //    if (tabelaFrete == null && tomador.GrupoPessoas != null)
        //    {
        //        tabelasFrete = repTabelaFrete.BuscarPorGrupoPessoas(tomador.GrupoPessoas.Codigo, pagamentoTerceiro, tabelaFreteMinima);
        //        if (tabelasFrete.Count > 0)
        //            tabelaFrete = RetornarTabelaValidaValida(tabelasFrete, tomador.GrupoPessoas?.Codigo ?? 0, filiais, tiposOperacao, carregamento.Empresa?.Codigo ?? 0, dataParaVerificacaoVigencia);
        //    }

        //    if (tabelaFrete == null)
        //    {
        //        tabelaFrete = repTabelaFrete.BuscarPadrao(pagamentoTerceiro);
        //        if (tabelaFrete != null)
        //        {
        //            tabelasFrete.Clear();
        //            tabelasFrete.Add(tabelaFrete);
        //            tabelaFrete = RetornarTabelaValidaValida(tabelasFrete, tomador.GrupoPessoas?.Codigo ?? 0, filiais, tiposOperacao, carregamento.Empresa?.Codigo ?? 0, dataParaVerificacaoVigencia);
        //        }
        //    }

        //    if (tabelaFrete != null)
        //    {
        //        tabelasFrete.Clear();
        //        tabelasFrete.Add(tabelaFrete);
        //    }
        //    else
        //    {
        //        tabelasFrete.Clear();
        //    }

        //    return tabelasFrete;
        //}

        //private static Dominio.Entidades.Embarcador.Frete.TabelaFrete RetornarTabelaValidaValida(List<Dominio.Entidades.Embarcador.Frete.TabelaFrete> tabelas, int grupoTomador, List<int> filiais, List<int> tiposOperacao, int empresa, DateTime dataVigencia)
        //{
        //    Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaValida = null;
        //    foreach (Dominio.Entidades.Embarcador.Frete.TabelaFrete tabela in tabelas)
        //    {
        //        bool valido = true;
        //        if (tabela.Transportadores != null && tabela.Transportadores.Count > 0)
        //        {
        //            if (!tabela.Transportadores.Any(obj => obj.Codigo == empresa))
        //                valido = false;
        //        }

        //        if (tabela.TiposOperacao != null && tabela.TiposOperacao.Count > 0)
        //        {
        //            foreach (int tipoOperacao in tiposOperacao)
        //            {
        //                if (!tabela.TiposOperacao.Any(obj => obj.Codigo == tipoOperacao))
        //                    valido = false;
        //            }
        //        }

        //        //if (tabela.TiposOperacao != null && tabela.TiposOperacao.Count > 0 && !tabela.TiposOperacao.All(tp => tiposOperacao.Contains(tp.Codigo)))
        //        //    valido = false;
        //        if (tabela.Filiais != null && tabela.Filiais.Count > 0)
        //        {
        //            if (!tabela.Filiais.Any(obj => filiais.Contains(obj.Codigo)))
        //                valido = false;
        //        }
        //        if (tabela.GrupoPessoas != null && tabela.GrupoPessoas.Codigo != grupoTomador)
        //            valido = false;

        //        //if (tabela.ContratoFreteTransportador != null)
        //        //{
        //        //    if (tabela.ContratoFreteTransportador.Transportador != null && tabela.ContratoFreteTransportador.Transportador.Codigo != empresa)
        //        //        valido = false;

        //        //    if (tabela.ContratoFreteTransportador.Filiais != null && tabela.ContratoFreteTransportador.Filiais.Count > 0 && !tabela.ContratoFreteTransportador.Filiais.Any(obj => obj.Filial.Codigo == filial))
        //        //        valido = false;

        //        //    if (tabela.ContratoFreteTransportador.DataInicial > dataVigencia.Date || tabela.ContratoFreteTransportador.DataFinal < dataVigencia.Date)
        //        //        valido = false;
        //        //}

        //        if (valido)
        //        {
        //            tabelaValida = tabela;
        //            break;
        //        }

        //    }
        //    return tabelaValida;
        //}

        //private Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete ObterParametrosCalculoFretePorCarga(Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete, Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, bool calculoFreteFilialEmissora, Repositorio.UnitOfWork unidadeTrabalho, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        //{
        //    Servicos.Embarcador.Carga.FreteCliente svcFreteCliente = new Servicos.Embarcador.Carga.FreteCliente(stringConexao);
        //    Servicos.Embarcador.Carga.CTe svcCargaCTe = new CTe(StringConexao);

        //    Repositorio.Embarcador.Cargas.CargaPedidoQuantidades repCargaPedidoQuantidades = new Repositorio.Embarcador.Cargas.CargaPedidoQuantidades(unidadeTrabalho);
        //    Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unidadeTrabalho);
        //    Repositorio.Embarcador.Cargas.CargaPercurso repCargaPercurso = new Repositorio.Embarcador.Cargas.CargaPercurso(unidadeTrabalho);
        //    Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unidadeTrabalho);
        //    Repositorio.Embarcador.Cargas.CargaLocaisPrestacao repCargaLocaisPrestacao = new Repositorio.Embarcador.Cargas.CargaLocaisPrestacao(unidadeTrabalho);
        //    Repositorio.Embarcador.Frete.DestinoPrioritarioCalculoFreteLocalidade repDestinoPrioritarioCalculoFreteLocalidade = new Repositorio.Embarcador.Frete.DestinoPrioritarioCalculoFreteLocalidade(unidadeTrabalho);

        //    List<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao> cargaLocaisPrestacao = new List<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao>();
        //    int distancia = 0;
        //    decimal peso = 0;
        //    decimal valorTotalNotasFiscais = 0;

        //    List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga> quantidadesCarga = new List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga>();
        //    Dominio.Entidades.RotaFrete rotaFrete = null;

        //    List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> tiposDeCarga = (from obj in cargas select obj.TipoDeCarga).Distinct().ToList();

        //    foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargas)
        //    {
        //        cargaLocaisPrestacao.AddRange(repCargaLocaisPrestacao.BuscarPorCarga(carga.Codigo));

        //        if (carga.Rota != null && carga.Rota.Quilometros > 0)
        //            distancia += (int)carga.Rota.Quilometros;
        //        else
        //            distancia += repCargaPercurso.ConsultarDistanciaTotalPorCarga(carga.Codigo);

        //        List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades> cargaPedidoQuantidadesPorCarga = repCargaPedidoQuantidades.BuscarPorCarga(carga.Codigo);
        //        foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades quantidadeCarga in cargaPedidoQuantidadesPorCarga)
        //        {
        //            Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga quantidadeCargaCTe = (from obj in quantidadesCarga where obj.Unidade == quantidadeCarga.Unidade select obj).FirstOrDefault();

        //            if (quantidadeCargaCTe == null)
        //            {
        //                quantidadeCargaCTe = new Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga();
        //                quantidadeCargaCTe.Unidade = quantidadeCarga.Unidade;
        //                quantidadeCargaCTe.Quantidade = quantidadeCarga.Quantidade;
        //                quantidadesCarga.Add(quantidadeCargaCTe);
        //            }
        //            else
        //                quantidadeCargaCTe.Quantidade += quantidadeCarga.Quantidade;

        //        }
        //        valorTotalNotasFiscais += repPedidoXMLNotaFiscal.BuscarTotalPorCarga(carga.Codigo);

        //        if (rotaFrete == null)
        //            rotaFrete = carga.Rota;
        //    }
        //    peso = svcFreteCliente.ObterQuilosTotaisParaQuilos(quantidadesCarga);

        //    if (peso <= 0m)
        //        peso = tabelaFrete.UtilizarPesoLiquido ? cargaPedidos.Sum(o => o.PesoLiquido) : cargaPedidos.Sum(o => o.Peso);

        //    int quantidadeNotasFiscais = repPedidoXMLNotaFiscal.ContarPorCargaPedido(cargaPedidos.Select(o => o.Codigo));

        //    int quantidadeEntregas = 0;
        //    if (tipoServicoMultisoftware == TipoServicoMultisoftware.MultiTMS)
        //        quantidadeEntregas = cargaPedidos.Sum(o => o.Pedido.QtdEntregas);
        //    else
        //    {
        //        Servicos.Embarcador.Carga.CargaDadosSumarizados serCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unidadeTrabalho);
        //        quantidadeEntregas = serCargaDadosSumarizados.BuscarNumeroDeEntregasPorPedido(cargaPedidos, unidadeTrabalho);
        //    }

        //    List<Dominio.Entidades.Localidade> origens = new List<Dominio.Entidades.Localidade>();
        //    List<Dominio.Entidades.Localidade> destinos = new List<Dominio.Entidades.Localidade>();

        //    if (tabelaFrete.UtilizarParticipantePedidoParaCalculo)
        //    {
        //        origens = (from obj in cargaPedidos where obj.Pedido?.Origem != null select obj.Pedido.Origem).Distinct().ToList();
        //        destinos = (from obj in cargaPedidos where obj.Pedido?.Destino != null select obj.Pedido.Destino).Distinct().ToList();
        //    }
        //    else
        //    {
        //        origens = (from obj in cargaLocaisPrestacao select obj.LocalidadeInicioPrestacao).Distinct().ToList();
        //        destinos = (from obj in cargaLocaisPrestacao select obj.LocalidadeTerminoPrestacao).Distinct().ToList();
        //    }

        //    if (tabelaFrete.CalcularFreteDestinoPrioritario)
        //    {
        //        Dominio.Entidades.Embarcador.Frete.DestinoPrioritarioCalculoFreteLocalidade localidadePrioritaria = repDestinoPrioritarioCalculoFreteLocalidade.ValidarPorTabelaFreteELocalidades(tabelaFrete.Codigo, (from o in destinos select o.Codigo).ToList());
        //        if (localidadePrioritaria != null)
        //            destinos = new List<Dominio.Entidades.Localidade>() { localidadePrioritaria.Localidade };
        //    }


        //    List<Dominio.Entidades.Cliente> remetentes = new List<Dominio.Entidades.Cliente>();
        //    List<Dominio.Entidades.Cliente> destinatarios = new List<Dominio.Entidades.Cliente>();

        //    Dictionary<Dominio.Entidades.ModeloDocumentoFiscal, int> quantidadesDocumentosEmitir = null;


        //    foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
        //    {
        //        if (tabelaFrete.UtilizarParticipantePedidoParaCalculo || (!svcCargaCTe.VerificarSePercursoDestinoSeraPorNota(cargaPedido.TipoRateio, cargaPedido.TipoEmissaoCTeParticipantes) ||
        //            cargaPedido.TipoRateio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorPedidoAgrupado))
        //        {
        //            if (cargaPedido.Recebedor != null && !(cargaPedido.Carga.TipoOperacao?.UtilizarRecebedorApenasComoParticipante ?? false))
        //                destinatarios.Add(cargaPedido.Recebedor);
        //            else if (cargaPedido.Pedido.Destinatario != null)
        //                destinatarios.Add(cargaPedido.Pedido.Destinatario);
        //        }
        //        else
        //        {
        //            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLs = repPedidoXMLNotaFiscal.BuscarPorCargaPedido(cargaPedido.Codigo);

        //            destinatarios = (from obj in pedidoXMLs where obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida select obj.XMLNotaFiscal.Destinatario).Distinct().ToList();
        //            destinatarios.AddRange((from obj in pedidoXMLs where obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Entrada select obj.XMLNotaFiscal.Emitente).Distinct().ToList());
        //        }

        //        if (tabelaFrete.UtilizarParticipantePedidoParaCalculo || (!svcCargaCTe.VerificarSePercursoOrigemSeraPorNota(cargaPedido.TipoRateio, cargaPedido.TipoEmissaoCTeParticipantes) ||
        //            cargaPedido.TipoRateio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorPedidoAgrupado))
        //        {
        //            if (cargaPedido.Expedidor != null)
        //                remetentes.Add(cargaPedido.Expedidor);
        //            if (cargaPedido.Pedido.Remetente != null)
        //                remetentes.Add(cargaPedido.Pedido.Remetente);
        //        }
        //        else
        //        {
        //            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLs = repPedidoXMLNotaFiscal.BuscarPorCargaPedido(cargaPedido.Codigo);

        //            remetentes = (from obj in pedidoXMLs where obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida select obj.XMLNotaFiscal.Emitente).Distinct().ToList();
        //            remetentes.AddRange((from obj in pedidoXMLs where obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Entrada select obj.XMLNotaFiscal.Destinatario).Distinct().ToList());
        //        }
        //    }

        //    remetentes = (from obj in remetentes select obj).Distinct().ToList();
        //    destinatarios = (from obj in destinatarios select obj).Distinct().ToList();
        //    origens = (from obj in origens select obj).Distinct().ToList();
        //    destinos = (from obj in destinos select obj).Distinct().ToList();

        //    Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosCalculoFrete parametrosCalculoFrete = new ParametrosCalculoFrete()
        //    {
        //        DataColeta = cargaPedidos.Select(o => o.Pedido.DataInicialColeta).Min(),
        //        DataFinalViagem = cargaPedidos.Select(o => o.Pedido.DataFinalViagemFaturada).Max(),
        //        DataInicialViagem = cargaPedidos.Select(o => o.Pedido.DataInicialViagemFaturada).Min(),
        //        DataVigencia = carregamento.DataCarregamentoCarga,
        //        Desistencia = false,
        //        DespachoTransitoAduaneiro = cargaPedidos.Any(o => o.Pedido.DespachoTransitoAduaneiro),
        //        Destinatarios = destinatarios,
        //        Destinos = destinos,
        //        Distancia = distancia,
        //        Empresa = carregamento.Empresa,//!calculoFreteFilialEmissora ? carregamento.Empresa : carregamento.EmpresaFilialEmissora,
        //        EscoltaArmada = cargaPedidos.Any(o => o.Pedido.EscoltaArmada),
        //        QuantidadeEscolta = cargaPedidos.Where(o => o.Pedido.EscoltaArmada).Sum(o => o.Pedido.QtdEscolta),
        //        Filial = null, //carga.Filial,
        //        GerenciamentoRisco = cargaPedidos.Any(o => o.Pedido.GerenciamentoRisco),
        //        GrupoPessoas = null, //carga.GrupoPessoaPrincipal,
        //        ModelosUtilizadosEmissao = cargaPedidos.Select(o => o.ModeloDocumentoFiscal).Distinct().ToList(),
        //        ModeloVeiculo = carregamento.ModeloVeicularCarga,
        //        NecessarioReentrega = cargaPedidos.Any(o => o.Pedido.NecessarioReentrega),
        //        NumeroAjudantes = cargaPedidos.Sum(o => o.Pedido.QtdAjudantes),
        //        NumeroEntregas = quantidadeEntregas,
        //        NumeroPallets = cargaPedidos.Sum(o => o.Pedido.NumeroPaletes + o.Pedido.NumeroPaletesFracionado),
        //        Origens = origens,
        //        PercentualDesistencia = 0,//carga.PercentualDesistencia,
        //        Peso = peso,
        //        PesoCubado = cargaPedidos.Sum(o => o.Pedido.PesoCubado),
        //        PesoPaletizado = cargaPedidos.Sum(o => o.Pedido.PesoTotalPaletes),
        //        PossuiRestricaoTrafego = remetentes.Any(o => o.PossuiRestricaoTrafego) || destinatarios.Any(o => o.PossuiRestricaoTrafego),
        //        QuantidadeNotasFiscais = quantidadeNotasFiscais,
        //        QuantidadeEmissoesPorModeloDocumento = quantidadesDocumentosEmitir,
        //        Quantidades = (from obj in quantidadesCarga
        //                       select new ParametrosCalculoFreteQuantidade()
        //                       {
        //                           Quantidade = obj.Quantidade,
        //                           UnidadeMedida = obj.Unidade
        //                       }).ToList(),
        //        Rastreado = cargaPedidos.Any(o => o.Pedido.Rastreado),
        //        Remetentes = remetentes,
        //        Rota = rotaFrete,
        //        RotasDinamicas = cargaPedidos.Where(o => o.Pedido.RotaFrete != null).Select(o => o.Pedido.RotaFrete).ToList(),
        //        CodigosRotasFixas = cargaPedidos.Where(o => o.RotasFretes != null).Select(o => o.RotasFretes.Select(r => r.Codigo)).SelectMany(x => x).ToList(),
        //        TipoCarga = tiposDeCarga.Count == 1 ? tiposDeCarga.FirstOrDefault() : null, //carga.TipoDeCarga,
        //        TipoOperacao = null,//carregamento.TipoOperacao,
        //        Tomador = cargaPedidos.FirstOrDefault().ObterTomador(),
        //        ValorNotasFiscais = valorTotalNotasFiscais,
        //        Veiculo = carregamento.Veiculo,
        //        Volumes = (from obj in quantidadesCarga where obj.Unidade == Dominio.Enumeradores.UnidadeMedida.UN select obj.Quantidade).Sum()
        //    };

        //    return parametrosCalculoFrete;
        //}

    }
}
