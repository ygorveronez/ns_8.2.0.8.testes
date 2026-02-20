using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Carga
{
    public class FreteCTeSubcontratacao : ServicoBase
    {
        public FreteCTeSubcontratacao(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        public Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete BuscarTabelaFreteSubcontratado(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, bool apenasVerificar, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {

            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Frete.TabelaFrete repTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFrete(unitOfWork);
            Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas repModalidadeTransportadoraPessoas = new Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaTabelaFreteSubContratacao repCargaTabelaFreteSubContratacao = new Repositorio.Embarcador.Cargas.CargaTabelaFreteSubContratacao(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            // Servicos.Embarcador.Carga.ICMS serICMS = new ICMS(StringConexao);

            Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete = null;
            Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete retornoFrete = new Dominio.ObjetosDeValor.Embarcador.Frete.RetornoDadosFrete();
            Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteSubContratacao cargaTabelaFreteSubContratacao = repCargaTabelaFreteSubContratacao.BuscarPorCarga(carga.Codigo);

            if (!apenasVerificar)
            {

                decimal percentualCobranca = 0;

                Dominio.Entidades.Cliente remetente = (from obj in cargaPedidos select obj.Pedido.Remetente).FirstOrDefault();

                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = (from obj in cargaPedidos select obj.Pedido).FirstOrDefault();
                if (pedido.SubContratante != null)
                {
                    if (remetente.GrupoPessoas != null)
                    {
                        tabelaFrete = repTabelaFrete.BuscarPorGrupoPessoa(remetente.GrupoPessoas.Codigo, carga.TipoOperacao, false, false, false, carga.Terceiro?.CPF_CNPJ, null);
                        if (tabelaFrete == null)
                            tabelaFrete = repTabelaFrete.BuscarPorGrupoPessoa(remetente.GrupoPessoas.Codigo, false, false, false);
                        if (tabelaFrete != null)
                        {
                            Dominio.Entidades.Embarcador.Frete.SubcontratacaoTabelaFrete subContratacao = (from obj in tabelaFrete.Subcontratacoes where obj.Pessoa.Codigo == pedido.SubContratante.Codigo select obj).FirstOrDefault();
                            if (subContratacao != null && subContratacao.PercentualCobranca > 0)
                                percentualCobranca = subContratacao.PercentualCobranca;
                            else
                                tabelaFrete = null;
                        }
                    }

                    if (percentualCobranca == 0)
                    {
                        Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas modalidade = (from obj in pedido.SubContratante.Modalidades where obj.TipoModalidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalidade.TransportadorTerceiro select obj).FirstOrDefault();
                        if (modalidade != null)
                        {
                            Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTransportadoraPessoas = repModalidadeTransportadoraPessoas.BuscarPorModalidade(modalidade.Codigo);
                            if (modalidadeTransportadoraPessoas != null)
                                percentualCobranca = modalidadeTransportadoraPessoas.PercentualCobranca;
                        }
                        else if (tabelaFrete != null && tabelaFrete.PercentualCobrancaVeiculoFrotaTerceiros > 0)
                        {
                            percentualCobranca = tabelaFrete.PercentualCobrancaVeiculoFrotaTerceiros;
                        }
                    }
                }


                if (percentualCobranca > 0)
                {
                    bool inserir = false;
                    if (cargaTabelaFreteSubContratacao == null)
                    {
                        inserir = true;
                        cargaTabelaFreteSubContratacao = new Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteSubContratacao();
                        cargaTabelaFreteSubContratacao.Carga = carga;
                    }
                    else
                    {
                        cargaTabelaFreteSubContratacao.TabelaFrete = null;
                        cargaTabelaFreteSubContratacao.TransportadorTerceiro = null;
                    }
                    cargaTabelaFreteSubContratacao.TabelaFrete = tabelaFrete;
                    cargaTabelaFreteSubContratacao.TransportadorTerceiro = pedido.SubContratante;
                    cargaTabelaFreteSubContratacao.ValorTotalRecebeCTeParaSubContratacao = 0;
                    cargaTabelaFreteSubContratacao.ValorTotalICMSCTeParaSubContratacao = 0;
                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                    {
                        cargaPedido.Pedido.ObservacaoCTe = "";
                        List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> pedidoCTeParaSubContratacao = repPedidoCTeParaSubContratacao.BuscarPorCargaPedido(cargaPedido.Codigo);
                        decimal valorTotalCteParaSubContratacao = pedidoCTeParaSubContratacao.Sum(obj => obj.CTeTerceiro.ValorAReceber);
                        decimal valorTotalICMSParaSubContratacao = pedidoCTeParaSubContratacao.Sum(obj => obj.CTeTerceiro.ValorICMS);
                        cargaTabelaFreteSubContratacao.ValorTotalRecebeCTeParaSubContratacao += valorTotalCteParaSubContratacao;
                        cargaTabelaFreteSubContratacao.ValorTotalICMSCTeParaSubContratacao += valorTotalICMSParaSubContratacao;

                        cargaPedido.ValorFrete = (valorTotalCteParaSubContratacao) * (percentualCobranca / 100);
                        cargaPedido.ValorFreteAPagar = cargaPedido.ValorFrete;
                        cargaPedido.ValorICMS = 0;
                        cargaPedido.PercentualAliquota = 0;
                        cargaPedido.CST = "40";
                        cargaPedido.CFOP = pedidoCTeParaSubContratacao.FirstOrDefault().CTeTerceiro.CFOP;
                        repCargaPedido.Atualizar(cargaPedido);
                    }

                    cargaTabelaFreteSubContratacao.ValorFrete = (cargaTabelaFreteSubContratacao.ValorTotalRecebeCTeParaSubContratacao) * (percentualCobranca / 100);
                    cargaTabelaFreteSubContratacao.PercentualCobrado = percentualCobranca;

                    carga.TipoFreteEscolhido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Tabela;
                    carga.ValorFreteOperador = cargaTabelaFreteSubContratacao.ValorFrete;
                    carga.ValorFreteAPagar = cargaTabelaFreteSubContratacao.ValorFrete;
                    carga.ValorFrete = cargaTabelaFreteSubContratacao.ValorFrete;
                    carga.ValorFreteTabelaFrete = cargaTabelaFreteSubContratacao.ValorFrete;
                    carga.ValorICMS = 0;

                    carga.PossuiPendencia = false;

                    Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);

                    bool existeConfirmacaoEtapaFreteNoFluxoNotaAposFretePorTipoOperacao = repositorioConfiguracaoGeralCarga.ExisteConfirmacaoEtapaFreteNoFluxoNotaAposFretePorTipoOperacao() && (carga.TipoOperacao?.ExigeConformacaoFreteAntesEmissao ?? false);

                    if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador && !carga.ExigeNotaFiscalParaCalcularFrete && !existeConfirmacaoEtapaFreteNoFluxoNotaAposFretePorTipoOperacao)
                        carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgTransportador;
                    else
                        carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete;

                    carga.MotivoPendenciaFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoPendenciaFrete.NenhumPendencia;
                    carga.MotivoPendencia = "";
                    repCarga.Atualizar(carga);

                    if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete)
                        Servicos.Log.TratarErro("Atualizou a situação para calculo frete 7 Carga " + carga.CodigoCargaEmbarcador, "AtualizouSituacaoCalculoFrete");

                    if (inserir)
                        repCargaTabelaFreteSubContratacao.Inserir(cargaTabelaFreteSubContratacao);
                    else
                        repCargaTabelaFreteSubContratacao.Atualizar(cargaTabelaFreteSubContratacao);
                }
                else
                {
                    carga.ValorFreteAPagar = 0;
                    carga.ValorFrete = 0;
                    carga.ValorFreteTabelaFrete = 0;
                    carga.ValorICMS = 0;
                    carga.PossuiPendencia = true;
                    repCarga.Atualizar(carga);
                }
            }

            if (cargaTabelaFreteSubContratacao != null)
            {
                Dominio.ObjetosDeValor.Embarcador.Frete.FretePercentualSubcontratacao fretePercentualSubcontratacao = new Dominio.ObjetosDeValor.Embarcador.Frete.FretePercentualSubcontratacao();
                string descricaoTabelaFrete = "Utilizado o percentual de " + cargaTabelaFreteSubContratacao.PercentualCobrado.ToString("n2") + " % sobre o valor dos CT-es pré acordado com " + cargaTabelaFreteSubContratacao.TransportadorTerceiro.Nome;
                if (cargaTabelaFreteSubContratacao.TabelaFrete != null)
                {
                    descricaoTabelaFrete += " na tabela de frete " + cargaTabelaFreteSubContratacao.TabelaFrete.Descricao;
                }
                descricaoTabelaFrete += " para cálculo do frete";

                fretePercentualSubcontratacao.TabelaFrete = descricaoTabelaFrete;
                fretePercentualSubcontratacao.valorTotalCTesParaSubContratacao = cargaTabelaFreteSubContratacao.ValorTotalICMSCTeParaSubContratacao;
                fretePercentualSubcontratacao.valorTotalICMSCTesParaSubContratacao = cargaTabelaFreteSubContratacao.ValorTotalRecebeCTeParaSubContratacao;
                fretePercentualSubcontratacao.percentualCobrado = cargaTabelaFreteSubContratacao.PercentualCobrado;
                fretePercentualSubcontratacao.valorFrete = cargaTabelaFreteSubContratacao.ValorFrete;
                retornoFrete.situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.FreteValido;
                retornoFrete.dadosRetornoTipoFrete = fretePercentualSubcontratacao;
                retornoFrete.tipoTabelaFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFrete.tabelaSubContratacao;

                if (!apenasVerificar)
                {
                    Servicos.Embarcador.Carga.RateioFrete serFreteRateio = new RateioFrete(unitOfWork);
                    serFreteRateio.RatearValorDoFrenteEntrePedidos(carga, cargaPedidos, configuracao, false, unitOfWork, tipoServicoMultisoftware);
                }
            }
            else
            {
                retornoFrete.situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoDadosFrete.ProblemaCalcularFrete;
                retornoFrete.mensagem = "Não existe uma tabela configurada para essa subcontratação.";
            }

            return retornoFrete;
        }
    }
}