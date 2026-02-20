using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Servicos.Embarcador.Acerto
{
    public class AcertoViagem : ServicoBase
    {

        public AcertoViagem(Repositorio.UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken) { }

        #region Métodos Públicos

        public void InserirLogAcerto(Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem, Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoLogAcertoViagem acao, Dominio.Entidades.Usuario usuario)
        {
            Repositorio.Embarcador.Acerto.AcertoLog repAcertoLog = new Repositorio.Embarcador.Acerto.AcertoLog(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Acerto.AcertoLog log = new Dominio.Entidades.Embarcador.Acerto.AcertoLog();

            log.AcertoViagem = acertoViagem;
            log.DataHora = DateTime.Now;
            log.TipoAcao = acao;
            log.Usuario = usuario;

            repAcertoLog.Inserir(log);
        }

        public List<Dominio.Entidades.Embarcador.Cargas.Carga> InserirCargaAcerto(Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem, Repositorio.UnitOfWork unidadeDeTrabalho, string situacaoesCarga)
        {
            Servicos.Log.GravarInfo(acertoViagem?.Codigo.ToString() + " Inicio InserirCargaAcerto " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            List<int> listaSituacaoCarga = new List<int>();
            if (!string.IsNullOrEmpty(situacaoesCarga))
            {
                var lista = situacaoesCarga.Split(';');
                foreach (var situacao in lista)
                    listaSituacaoCarga.Add(int.Parse(Utilidades.String.OnlyNumbers(situacao)));
            }

            List<Dominio.Entidades.Embarcador.Cargas.Carga> listaRetorno = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeDeTrabalho);
            Repositorio.Embarcador.Acerto.AcertoCarga repAcertoCarga = new Repositorio.Embarcador.Acerto.AcertoCarga(unidadeDeTrabalho);
            Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unidadeDeTrabalho);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            IList<int> listaCarga = repCarga.BuscarCargasPorSituacaoMotoristaDataVerificaCargaSemAcerto(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada, acertoViagem.Motorista, acertoViagem.DataInicial, acertoViagem.DataFinal.HasValue ? acertoViagem.DataFinal.Value : DateTime.Now.Date, listaSituacaoCarga, configuracaoTMS.NaoAdicionarCargasTransbordoAcertoViagem);
            
            for (int i = 0; i < listaCarga.Count(); i++)
            {
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(listaCarga[i]);

                if (carga != null)
                {
                    listaRetorno.Add(carga);

                    Dominio.Entidades.Embarcador.Acerto.AcertoCarga acertoCarga = new Dominio.Entidades.Embarcador.Acerto.AcertoCarga();
                    acertoCarga.AcertoViagem = acertoViagem;
                    acertoCarga.Carga = carga;
                    acertoCarga.PedagioAcerto = 0;
                    acertoCarga.PedagioAcertoCredito = 0;
                    acertoCarga.PercentualAcerto = 100;
                    acertoCarga.CargaFracionada = false;
                    acertoCarga.ValorBonificacaoCliente = 0;
                    acertoCarga.ValorBrutoCarga = repCarga.BuscarValorFreteAReceberConhecimentos(carga.Codigo);
                    acertoCarga.ValorICMSCarga = repCarga.BuscarValorICMSConhecimentos(carga.Codigo);

                    repAcertoCarga.Inserir(acertoCarga);

                    Servicos.Embarcador.Pedido.Pedido.AtualizarSituacaoPlanejamentoPedidoTMS(acertoCarga.Carga, null, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPlanejamentoPedidoTMS.CargaPossuiAcertoAberto, unidadeDeTrabalho);
                }
            }

            Servicos.Log.GravarInfo(acertoViagem?.Codigo.ToString() + " Fim InserirCargaAcerto " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            return listaRetorno;
        }

        public List<Dominio.Entidades.Veiculo> InserirVeiculoAcerto(Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Servicos.Log.GravarInfo(acertoViagem?.Codigo.ToString() + " Inicio InserirVeiculoAcerto " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            Repositorio.Embarcador.Acerto.AcertoVeiculo repAcertoVeiculo = new Repositorio.Embarcador.Acerto.AcertoVeiculo(unidadeDeTrabalho);

            List<Dominio.Entidades.Veiculo> listaRetorno = new List<Dominio.Entidades.Veiculo>();

            List<Dominio.Entidades.Veiculo> listaVeiculo = repAcertoVeiculo.BuscarVeiculosAcertoCarga(acertoViagem.Codigo);
            listaVeiculo.AddRange(repAcertoVeiculo.BuscarVeiculosVinculadosAcertoCarga(acertoViagem.Codigo));

            listaVeiculo = listaVeiculo.Distinct().ToList();

            for (int i = 0; i < listaVeiculo.Count(); i++)
            {
                Dominio.Entidades.Veiculo veiculo = listaVeiculo[i];

                if (veiculo != null)
                {
                    listaRetorno.Add(veiculo);

                    Dominio.Entidades.Embarcador.Acerto.AcertoVeiculo acertoVeiculo = repAcertoVeiculo.BuscarPorAcertoEVeiculo(acertoViagem.Codigo, veiculo.Codigo);

                    if (acertoVeiculo == null)
                    {
                        acertoVeiculo = new Dominio.Entidades.Embarcador.Acerto.AcertoVeiculo
                        {
                            AcertoViagem = acertoViagem,
                            Veiculo = veiculo,
                            SegmentoVeiculo = acertoViagem.SegmentoVeiculo
                        };

                        repAcertoVeiculo.Inserir(acertoVeiculo);
                    }
                    else
                    {
                        acertoVeiculo.SegmentoVeiculo = acertoViagem.SegmentoVeiculo;

                        repAcertoVeiculo.Atualizar(acertoVeiculo);
                    }
                }
            }
            AtualizarVeiculosSegmento(acertoViagem, unidadeDeTrabalho);

            Servicos.Log.GravarInfo(acertoViagem?.Codigo.ToString() + " Fim InserirVeiculoAcerto " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");

            return listaRetorno;
        }

        public void InserirPegadioAcerto(Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem, Repositorio.UnitOfWork unidadeDeTrabalho, List<Dominio.Entidades.Veiculo> listaVeiculo, List<Dominio.Entidades.Embarcador.Cargas.Carga> listaCarga)
        {
            Servicos.Log.GravarInfo(acertoViagem?.Codigo.ToString() + " Inicio InserirPegadioAcerto " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            if (listaVeiculo.Count() == 0)
            {
                Servicos.Log.GravarInfo(acertoViagem?.Codigo.ToString() + " Fim InserirPegadioAcerto " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                return;
            }
            Repositorio.Embarcador.Pedagio.Pedagio repPedagio = new Repositorio.Embarcador.Pedagio.Pedagio(unidadeDeTrabalho);
            Repositorio.Embarcador.Acerto.AcertoPedagio repAcertoPedagio = new Repositorio.Embarcador.Acerto.AcertoPedagio(unidadeDeTrabalho);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            if (acertoViagem.Pedagios != null && acertoViagem.Pedagios.Count > 0)
            {
                Servicos.Log.GravarInfo(acertoViagem?.Codigo.ToString() + " Fim InserirPegadioAcerto " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                return;
            }

            if (listaCarga != null)
            {
                if (listaCarga.Count > 0)
                {
                    DateTime? maiorDataEncerramento = null;
                    Repositorio.Embarcador.Cargas.CargaRegistroEncerramento repCargaRegistroEncerramento = new Repositorio.Embarcador.Cargas.CargaRegistroEncerramento(unidadeDeTrabalho);
                    if (!configuracaoTMS.UtilizaMoedaEstrangeira)
                        maiorDataEncerramento = repCargaRegistroEncerramento.BuscarMaiorDataEncerramento(listaCarga.Select(o => o.Codigo).ToList());
                    //for (int a = 0; a < listaCarga.Count; a++)
                    //{
                    //Repositorio.Embarcador.Cargas.CargaRegistroEncerramento repCargaRegistroEncerramento = new Repositorio.Embarcador.Cargas.CargaRegistroEncerramento(unidadeDeTrabalho);
                    //Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramento cargaRegistroEncerramento = repCargaRegistroEncerramento.BuscarPorCarga(listaCarga[a].Codigo);

                    List<int> codigosVeiculos = listaVeiculo != null && listaVeiculo.Count > 0 ? listaVeiculo.Select(c => c.Codigo).ToList() : acertoViagem.Veiculos != null && acertoViagem.Veiculos.Count > 0 ? acertoViagem.Veiculos.Select(c => c.Veiculo.Codigo).ToList() : null;
                    List<Dominio.Entidades.Embarcador.Pedagio.Pedagio> listaPedagio = repPedagio.BuscarPedagioPorVeiculoDataSemAcerto(codigosVeiculos, acertoViagem.DataInicial, maiorDataEncerramento.HasValue ? maiorDataEncerramento.Value : acertoViagem.DataFinal.HasValue ? acertoViagem.DataFinal.Value : DateTime.Now.Date);

                    for (int i = 0; i < listaPedagio.Count(); i++)
                    {
                        if (acertoViagem.DataFinal.HasValue && listaPedagio[i].Data <= acertoViagem.DataFinal.Value.AddDays(1))
                        {
                            Dominio.Entidades.Embarcador.Acerto.AcertoPedagio acertoPedagio = new Dominio.Entidades.Embarcador.Acerto.AcertoPedagio();
                            acertoPedagio.AcertoViagem = acertoViagem;
                            acertoPedagio.Pedagio = listaPedagio[i];
                            acertoPedagio.LancadoManualmente = false;

                            repAcertoPedagio.Inserir(acertoPedagio);
                        }
                    }
                    //}
                }
                else
                {

                    List<int> codigosVeiculos = listaVeiculo != null && listaVeiculo.Count > 0 ? listaVeiculo.Select(c => c.Codigo).ToList() : acertoViagem.Veiculos != null && acertoViagem.Veiculos.Count > 0 ? acertoViagem.Veiculos.Select(c => c.Veiculo.Codigo).ToList() : null;
                    List<Dominio.Entidades.Embarcador.Pedagio.Pedagio> listaPedagio = repPedagio.BuscarPedagioPorVeiculoDataSemAcerto(codigosVeiculos, acertoViagem.DataInicial, acertoViagem.DataFinal.HasValue ? acertoViagem.DataFinal.Value : DateTime.Now.Date);

                    for (int i = 0; i < listaPedagio.Count(); i++)
                    {
                        if (acertoViagem.DataFinal.HasValue && listaPedagio[i].Data <= acertoViagem.DataFinal.Value.AddDays(1))
                        {
                            Dominio.Entidades.Embarcador.Acerto.AcertoPedagio acertoPedagio = new Dominio.Entidades.Embarcador.Acerto.AcertoPedagio();
                            acertoPedagio.AcertoViagem = acertoViagem;
                            acertoPedagio.Pedagio = listaPedagio[i];
                            acertoPedagio.LancadoManualmente = false;

                            repAcertoPedagio.Inserir(acertoPedagio);
                        }
                    }
                }
            }

            Servicos.Log.GravarInfo(acertoViagem?.Codigo.ToString() + " Fim InserirPegadioAcerto " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");

        }

        public void InserirAbastecimentoAcerto(Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem, Repositorio.UnitOfWork unidadeDeTrabalho, List<Dominio.Entidades.Veiculo> listaVeiculo, List<Dominio.Entidades.Embarcador.Cargas.Carga> listaCarga, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Servicos.Log.GravarInfo(acertoViagem?.Codigo.ToString() + " Inicio InserirAbastecimentoAcerto " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");

            if (listaVeiculo.Count() == 0)
            {
                Servicos.Log.GravarInfo(acertoViagem?.Codigo.ToString() + " Fim InserirAbastecimentoAcerto " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                return;
            }
            Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(unidadeDeTrabalho);
            Repositorio.Embarcador.Acerto.AcertoAbastecimento repAcertoAbastecimento = new Repositorio.Embarcador.Acerto.AcertoAbastecimento(unidadeDeTrabalho);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            if (acertoViagem.Abastecimentos != null && acertoViagem.Abastecimentos.Count > 0)
            {
                Servicos.Log.GravarInfo(acertoViagem?.Codigo.ToString() + " Fim InserirAbastecimentoAcerto " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                return;
            }

            if (listaCarga != null)
            {
                if (listaCarga.Count > 0)
                {
                    DateTime? maiorDataEncerramento = null;
                    Repositorio.Embarcador.Cargas.CargaRegistroEncerramento repCargaRegistroEncerramento = new Repositorio.Embarcador.Cargas.CargaRegistroEncerramento(unidadeDeTrabalho);
                    if (!configuracaoTMS.UtilizaMoedaEstrangeira)
                        maiorDataEncerramento = repCargaRegistroEncerramento.BuscarMaiorDataEncerramento(listaCarga.Select(o => o.Codigo).ToList());

                    List<int> codigosVeiculos = listaVeiculo != null && listaVeiculo.Count > 0 ? listaVeiculo.Select(c => c.Codigo).ToList() : acertoViagem.Veiculos != null && acertoViagem.Veiculos.Count > 0 ? acertoViagem.Veiculos.Select(c => c.Veiculo.Codigo).ToList() : null;
                    List<Dominio.Entidades.Abastecimento> listaAbastecimento = repAbastecimento.BuscarAbastecimentoPorVeiculoDataSemAcerto(codigosVeiculos, acertoViagem.DataInicial, maiorDataEncerramento.HasValue ? maiorDataEncerramento.Value : acertoViagem.DataFinal.HasValue ? acertoViagem.DataFinal.Value : DateTime.Now.Date);

                    for (int i = 0; i < listaAbastecimento.Count(); i++)
                    {
                        Dominio.Entidades.Embarcador.Acerto.AcertoAbastecimento acertoAbastecimento = new Dominio.Entidades.Embarcador.Acerto.AcertoAbastecimento();
                        acertoAbastecimento.AcertoViagem = acertoViagem;
                        acertoAbastecimento.Abastecimento = listaAbastecimento[i];
                        acertoAbastecimento.LancadoManualmente = false;

                        repAcertoAbastecimento.Inserir(acertoAbastecimento);

                        if (acertoViagem.Motorista != null)
                        {
                            Dominio.Entidades.Abastecimento abastecimento = repAbastecimento.BuscarPorCodigo(listaAbastecimento[i].Codigo);
                            abastecimento.Motorista = acertoViagem.Motorista;
                            abastecimento.Integrado = false;

                            repAbastecimento.Atualizar(abastecimento);
                            Servicos.Auditoria.Auditoria.Auditar(auditado, abastecimento, null, "Atualizou o motorista via Acerto de Viagem.", unidadeDeTrabalho);
                        }
                    }
                }
                else
                {
                    List<int> codigosVeiculos = listaVeiculo != null && listaVeiculo.Count > 0 ? listaVeiculo.Select(c => c.Codigo).ToList() : acertoViagem.Veiculos != null && acertoViagem.Veiculos.Count > 0 ? acertoViagem.Veiculos.Select(c => c.Veiculo.Codigo).ToList() : null;
                    List<Dominio.Entidades.Abastecimento> listaAbastecimento = repAbastecimento.BuscarAbastecimentoPorVeiculoDataSemAcerto(codigosVeiculos, acertoViagem.DataInicial, acertoViagem.DataFinal.HasValue ? acertoViagem.DataFinal.Value : DateTime.Now.Date);

                    for (int i = 0; i < listaAbastecimento.Count(); i++)
                    {
                        Dominio.Entidades.Embarcador.Acerto.AcertoAbastecimento acertoAbastecimento = new Dominio.Entidades.Embarcador.Acerto.AcertoAbastecimento();
                        acertoAbastecimento.AcertoViagem = acertoViagem;
                        acertoAbastecimento.Abastecimento = listaAbastecimento[i];
                        acertoAbastecimento.LancadoManualmente = false;

                        repAcertoAbastecimento.Inserir(acertoAbastecimento);

                        if (acertoViagem.Motorista != null)
                        {
                            Dominio.Entidades.Abastecimento abastecimento = repAbastecimento.BuscarPorCodigo(listaAbastecimento[i].Codigo);
                            abastecimento.Motorista = acertoViagem.Motorista;
                            abastecimento.Integrado = false;

                            repAbastecimento.Atualizar(abastecimento);
                            Servicos.Auditoria.Auditoria.Auditar(auditado, abastecimento, null, "Atualizou o motorista via Acerto de Viagem.", unidadeDeTrabalho);
                        }
                    }
                }

                for (int i = 0; i < listaVeiculo.Count(); i++)
                {
                    LancarResumoAbastecimento(acertoViagem, listaVeiculo[i], Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Combustivel, unidadeDeTrabalho);
                    LancarResumoAbastecimento(acertoViagem, listaVeiculo[i], Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Arla, unidadeDeTrabalho);
                }
            }

            Servicos.Log.GravarInfo(acertoViagem?.Codigo.ToString() + " Fim InserirAbastecimentoAcerto " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");

        }

        public dynamic RetornaObjetoCompletoAcertoViagem(int codigo, Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapasAcertoViagem etapa, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Servicos.Log.GravarInfo(codigo.ToString() + " Inicio RetornaObjetoCompletoAcertoViagem " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");

            Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unidadeDeTrabalho);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeDeTrabalho);
            Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas repModalidadeFornecedor = new Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas(unidadeDeTrabalho);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unidadeDeTrabalho);
            Repositorio.Embarcador.Acerto.AcertoPedagio repAcertoPedagio = new Repositorio.Embarcador.Acerto.AcertoPedagio(unidadeDeTrabalho);
            Repositorio.Embarcador.Logistica.GuaritaTMS repGuarita = new Repositorio.Embarcador.Logistica.GuaritaTMS(unidadeDeTrabalho);
            Repositorio.Embarcador.Pallets.DevolucaoPallet repDevolucao = new Repositorio.Embarcador.Pallets.DevolucaoPallet(unidadeDeTrabalho);
            Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unidadeDeTrabalho);

            Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem = repAcertoViagem.BuscarPorCodigo(codigo);

            Servicos.Embarcador.Carga.CargaDadosSumarizados serCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unidadeDeTrabalho);
            DateTime? dataEntradaGuarita = null;
            if (!acertoViagem.DataFinal.HasValue && !configuracaoTMS.DesabilitarSaldoViagemAcerto && !acertoViagem.DataFinalGuarita.HasValue)
            {
                Dominio.Entidades.Embarcador.Logistica.GuaritaTMS guarita = repGuarita.BuscarUltimoEntradaMotorista(acertoViagem.Motorista?.Codigo ?? 0);
                if (guarita != null && (!dataEntradaGuarita.HasValue || guarita.DataSaidaEntrada > dataEntradaGuarita.Value))
                    dataEntradaGuarita = guarita.DataSaidaEntrada;
            }
            if (dataEntradaGuarita.HasValue && dataEntradaGuarita.Value > DateTime.MinValue && dataEntradaGuarita.Value > acertoViagem.DataInicial && !acertoViagem.DataFinalGuarita.HasValue)
            {
                acertoViagem.DataFinalGuarita = dataEntradaGuarita;
                acertoViagem.DataFinal = dataEntradaGuarita;
                repAcertoViagem.Atualizar(acertoViagem);
                Servicos.Auditoria.Auditoria.Auditar(auditado, acertoViagem, null, "Buscou a data final a partir da última guarita de entrada encontrada.", unidadeDeTrabalho);
            }

            Repositorio.Embarcador.Configuracoes.ConfiguraoAcertoViagem repConfiguraoAcertoViagem = new Repositorio.Embarcador.Configuracoes.ConfiguraoAcertoViagem(unidadeDeTrabalho);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoAcertoViagem repConfiguracaoFinanceiraContratoAcertoViagem = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoAcertoViagem(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoAcertoViagem configuracaoAcertoViagem = repConfiguracaoFinanceiraContratoAcertoViagem.BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguraoAcertoViagem configuraoAcertoViagem = repConfiguraoAcertoViagem.BuscarPrimeiroRegistro();
            bool contemConfiguracao = configuracaoAcertoViagem != null && configuracaoAcertoViagem.GerarMovimentoAutomaticoNaGeracaoAcertoViagem ? true : false;
            bool visualizarPalletsCanhotosNasCargas = configuraoAcertoViagem?.VisualizarPalletsCanhotosNasCargas ?? false;

            Servicos.Log.GravarInfo(codigo.ToString() + " Fim RetornaObjetoCompletoAcertoViagem " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            return new
            {
                acertoViagem.Codigo,
                acertoViagem.CargaSalvo,
                acertoViagem.AbastecimentoSalvo,
                acertoViagem.PedagioSalvo,
                acertoViagem.DespesaSalvo,
                acertoViagem.DiariaSalvo,
                acertoViagem.OcorrenciaSalvo,
                acertoViagem.NumeroFrota,
                ListaAbastecimentos = (etapa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapasAcertoViagem.Abastecimentos || etapa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapasAcertoViagem.Todas) && acertoViagem.Abastecimentos != null ? (from obj in acertoViagem.Abastecimentos
                                                                                                                                                                                                                                                                orderby obj.Abastecimento.Data
                                                                                                                                                                                                                                                                select new
                                                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                                                    obj.Abastecimento.Codigo,
                                                                                                                                                                                                                                                                    DataHora = obj.Abastecimento.Data.HasValue ? obj.Abastecimento.Data.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty,
                                                                                                                                                                                                                                                                    obj.Abastecimento.Documento,
                                                                                                                                                                                                                                                                    obj.Abastecimento.Kilometragem,
                                                                                                                                                                                                                                                                    Litros = obj.Abastecimento.Litros.ToString("n2"),
                                                                                                                                                                                                                                                                    ValorUnitario = obj.Abastecimento.ValorUnitario.ToString("n4"),
                                                                                                                                                                                                                                                                    ValorTotal = obj.Abastecimento.ValorTotal.ToString("n2"),
                                                                                                                                                                                                                                                                    Placa = obj.Abastecimento.Veiculo != null ? obj.Abastecimento.Veiculo.Placa : string.Empty,
                                                                                                                                                                                                                                                                    CodigoVeiculo = obj.Abastecimento.Veiculo != null ? obj.Abastecimento.Veiculo.Codigo : 0,
                                                                                                                                                                                                                                                                    NomePosto = obj.Abastecimento.Posto != null ? obj.Abastecimento.Posto.Nome + " (" + obj.Abastecimento.Posto.CPF_CNPJ_Formatado + ")" : string.Empty,
                                                                                                                                                                                                                                                                    CodigoPosto = obj.Abastecimento.Posto != null ? obj.Abastecimento.Posto.CPF_CNPJ : 0,
                                                                                                                                                                                                                                                                    obj.Abastecimento.MoedaCotacaoBancoCentral,
                                                                                                                                                                                                                                                                    DataBaseCRT = obj.Abastecimento.DataBaseCRT.HasValue ? obj.Abastecimento.DataBaseCRT.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                                                                                                                                                                                                                                                                    ValorMoedaCotacao = obj.Abastecimento.ValorMoedaCotacao.ToString("n4"),
                                                                                                                                                                                                                                                                    ValorOriginalMoedaEstrangeira = obj.Abastecimento.ValorOriginalMoedaEstrangeira.ToString("n2")
                                                                                                                                                                                                                                                                }).ToList() : null,
                acertoViagem.AprovacaoAbastecimento,
                acertoViagem.AprovacaoPedagio,
                ListaBonificacoes = (etapa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapasAcertoViagem.Fechamento || etapa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapasAcertoViagem.Todas) && acertoViagem.Bonificacoes != null ? (from obj in acertoViagem.Bonificacoes
                                                                                                                                                                                                                                                        orderby obj.Codigo
                                                                                                                                                                                                                                                        select new
                                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                                            obj.Codigo,
                                                                                                                                                                                                                                                            obj.Motivo,
                                                                                                                                                                                                                                                            ValorBonificacao = obj.ValorBonificacao.ToString("n2")
                                                                                                                                                                                                                                                        }).ToList() : null,
                ListaCargas = (etapa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapasAcertoViagem.Cargas || etapa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapasAcertoViagem.Todas) && acertoViagem.Cargas != null ? (from obj in acertoViagem.Cargas
                                                                                                                                                                                                                                        orderby obj.Carga.DataCriacaoCarga descending
                                                                                                                                                                                                                                        select new
                                                                                                                                                                                                                                        {
                                                                                                                                                                                                                                            CodigoAcertoCarga = obj.Codigo,
                                                                                                                                                                                                                                            obj.Carga.Codigo,
                                                                                                                                                                                                                                            DataCriacaoCarga = obj.Carga.DataCriacaoCarga != null ? obj.Carga.DataCriacaoCarga : DateTime.Now.Date,
                                                                                                                                                                                                                                            Data = obj.Carga.DataCriacaoCarga != null ? obj.Carga.DataCriacaoCarga.ToString("dd/MM/yyyy") : string.Empty,
                                                                                                                                                                                                                                            obj.LancadoManualmente,
                                                                                                                                                                                                                                            Numero = obj.Carga.CodigoCargaEmbarcador,
                                                                                                                                                                                                                                            Placa = obj.Carga.Veiculo != null ? obj.Carga.Veiculo.Placa : string.Empty,
                                                                                                                                                                                                                                            Emitente = serCargaDadosSumarizados.ObterOrigemTMS(obj.Carga),
                                                                                                                                                                                                                                            Destino = serCargaDadosSumarizados.ObterDestinos(obj.Carga, true),
                                                                                                                                                                                                                                            PedagioAcerto = contemConfiguracao ? obj.DescricaoPedagioAcerto() : obj.DescricaoPedagioAcertoNaoPermiteAlterar(),
                                                                                                                                                                                                                                            PedagioAcertoCredito = contemConfiguracao ? obj.DescricaoPedagioAcertoCredito() : obj.DescricaoPedagioAcertoNaoPermiteAlterarCredito(),
                                                                                                                                                                                                                                            ValorFrete = repCarga.BuscarValorFreteAReceberConhecimentos(obj.Carga.Codigo).ToString("n2"),
                                                                                                                                                                                                                                            PercentualAcerto = obj.DescricaoPercentualAcerto(),
                                                                                                                                                                                                                                            ContemMaisDeUmMotorista = repCarga.ContemMaisDeUmMotorista(obj.Carga.Codigo),
                                                                                                                                                                                                                                            ContemMDFeEncerrado = repCarga.ContemMDFeEncerrado(obj.Carga.Codigo),
                                                                                                                                                                                                                                            obj.CargaFracionada,
                                                                                                                                                                                                                                            ValorBonificacaoCliente = obj.ValorBonificacaoCliente.ToString("n2"),
                                                                                                                                                                                                                                            ValorFracionada = obj.CargaFracionada ? obj.ValorBrutoCarga.ToString("n2") : "0,00",
                                                                                                                                                                                                                                            ValorBrutoCarga = obj.ValorBrutoCarga.ToString("n2"),
                                                                                                                                                                                                                                            ValorICMSCarga = obj.ValorICMSCarga.ToString("n2"),
                                                                                                                                                                                                                                            DT_RowColor = "#FFFFFF",
                                                                                                                                                                                                                                            SituacaoCanhotos = visualizarPalletsCanhotosNasCargas ? (repCanhoto.ConsultarSituacaoCanhotoCarga(obj.Carga.Codigo)?.FirstOrDefault()?.Situacao ?? "") : "",
                                                                                                                                                                                                                                            SituacaoPallets = visualizarPalletsCanhotosNasCargas ? (repDevolucao.ConsultarSituacaoPalletCarga(obj.Carga.Codigo)?.FirstOrDefault()?.Situacao ?? "") : ""
                                                                                                                                                                                                                                        }).ToList() : null,
                DataAcerto = acertoViagem.DataAcerto.ToString("dd/MM/yyyy"),
                DataFinal = acertoViagem.DataFinal.HasValue ? acertoViagem.DataFinal.Value.ToString("dd/MM/yyyy") : string.Empty,
                DataInicial = acertoViagem.DataInicial.ToString("dd/MM/yyyy"),
                DataHoraFinal = acertoViagem.DataFinal.HasValue ? acertoViagem.DataFinal.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty,
                DataHoraInicial = acertoViagem.DataInicial.ToString("dd/MM/yyyy HH:mm:ss"),
                ListaDescontos = (etapa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapasAcertoViagem.Fechamento || etapa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapasAcertoViagem.Todas) && acertoViagem.Descontos != null ? (from obj in acertoViagem.Descontos
                                                                                                                                                                                                                                                  orderby obj.Codigo
                                                                                                                                                                                                                                                  select new
                                                                                                                                                                                                                                                  {
                                                                                                                                                                                                                                                      obj.Codigo,
                                                                                                                                                                                                                                                      obj.Motivo,
                                                                                                                                                                                                                                                      ValorDesconto = obj.ValorDesconto.ToString("n2")
                                                                                                                                                                                                                                                  }).ToList() : null,
                acertoViagem.Etapa,
                Motorista = acertoViagem.Motorista != null ? new { Codigo = acertoViagem.Motorista.Codigo, Descricao = acertoViagem.Motorista.Nome } : null,
                SegmentoVeiculo = acertoViagem.SegmentoVeiculo != null ? new { Codigo = acertoViagem.SegmentoVeiculo.Codigo, Descricao = acertoViagem.SegmentoVeiculo.Descricao } : null,
                Cheque = acertoViagem.Cheque != null ? new { Codigo = acertoViagem.Cheque.Codigo, Descricao = acertoViagem.Cheque.Descricao } : null,
                NumeroAcerto = acertoViagem.Numero,
                acertoViagem.Observacao,
                Operador = acertoViagem.Operador != null ? new { Codigo = acertoViagem.Operador.Codigo, Descricao = acertoViagem.Operador.Nome } : null,
                ListaOutrasDespesas = (etapa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapasAcertoViagem.OutrasDespesas || etapa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapasAcertoViagem.Todas) && acertoViagem.OutrasDespesas != null ? (from obj in acertoViagem.OutrasDespesas
                                                                                                                                                                                                                                                                orderby obj.Data
                                                                                                                                                                                                                                                                select new
                                                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                                                    obj.Codigo,
                                                                                                                                                                                                                                                                    CodigoVeiculo = obj.Veiculo != null ? obj.Veiculo.Codigo : 0,
                                                                                                                                                                                                                                                                    DataHora = obj.Data.ToString("dd/MM/yyyy HH:mm:ss"),
                                                                                                                                                                                                                                                                    Produto = obj.Produto != null ? obj.Produto.Descricao : string.Empty,
                                                                                                                                                                                                                                                                    CodigoProduto = obj.Produto != null ? obj.Produto.Codigo : 0,
                                                                                                                                                                                                                                                                    Fornecedor = obj.Pessoa != null ? obj.Pessoa.Nome : obj.NomeFornecedor,
                                                                                                                                                                                                                                                                    CodigoFornecedor = obj.Pessoa != null ? obj.Pessoa.CPF_CNPJ : 0,
                                                                                                                                                                                                                                                                    Localidade = obj.Pessoa != null ? obj.Pessoa.Localidade.DescricaoCidadeEstado : string.Empty,
                                                                                                                                                                                                                                                                    PagoPor = obj.TipoPagamento == TipoPagamentoAcertoDespesa.Motorista ? "Motorista" : obj.TipoPagamento == TipoPagamentoAcertoDespesa.Empresa ? "Empresa" : obj.Pessoa != null ? obj.Pessoa.Modalidades != null && obj.Pessoa.Modalidades.Count > 0 && obj.Pessoa.Modalidades[0].Codigo > 0 ? repModalidadeFornecedor.BuscarPorCliente(obj.Pessoa.CPF_CNPJ) != null ? repModalidadeFornecedor.BuscarPorCliente(obj.Pessoa.CPF_CNPJ).PagoPorFatura ? "Faturamento" : "Motorista" : "Motorista" : "Motorista" : "Motorista",
                                                                                                                                                                                                                                                                    Quantidade = obj.Quantidade.ToString("n2"),
                                                                                                                                                                                                                                                                    Valor = obj.Valor.ToString("n2"),
                                                                                                                                                                                                                                                                    FaturamentoFornecedor = obj.TipoPagamento == TipoPagamentoAcertoDespesa.Empresa ? true : obj.Pessoa != null ? obj.Pessoa.Modalidades != null && obj.Pessoa.Modalidades.Count > 0 && obj.Pessoa.Modalidades[0].Codigo > 0 ? repModalidadeFornecedor.BuscarPorCliente(obj.Pessoa.CPF_CNPJ) != null ? repModalidadeFornecedor.BuscarPorCliente(obj.Pessoa.CPF_CNPJ).PagoPorFatura : false : false : false,
                                                                                                                                                                                                                                                                    obj.Observacao,
                                                                                                                                                                                                                                                                    obj.MoedaCotacaoBancoCentral,
                                                                                                                                                                                                                                                                    DataBaseCRT = obj.DataBaseCRT.HasValue ? obj.DataBaseCRT.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                                                                                                                                                                                                                                                                    ValorMoedaCotacao = obj.ValorMoedaCotacao.ToString("n4"),
                                                                                                                                                                                                                                                                    ValorOriginalMoedaEstrangeira = obj.ValorOriginalMoedaEstrangeira.ToString("n2"),
                                                                                                                                                                                                                                                                    obj.DespesaPagaPeloAdiantamento
                                                                                                                                                                                                                                                                }).ToList() : null,
                ListaPedagios = (etapa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapasAcertoViagem.Pedagios || etapa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapasAcertoViagem.Todas) && acertoViagem.Pedagios != null ? (from obj in acertoViagem.Pedagios
                                                                                                                                                                                                                                              where obj.Pedagio.TipoPedagio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedagio.Debito
                                                                                                                                                                                                                                              orderby obj.Pedagio.Data
                                                                                                                                                                                                                                              select new
                                                                                                                                                                                                                                              {
                                                                                                                                                                                                                                                  DataHoraPedagio = obj.Pedagio.Data,
                                                                                                                                                                                                                                                  CodigoAcertoPedagio = obj.Codigo,
                                                                                                                                                                                                                                                  obj.Pedagio.Codigo,
                                                                                                                                                                                                                                                  obj.LancadoManualmente,
                                                                                                                                                                                                                                                  DataHora = obj.Pedagio.Data.ToString("dd/MM/yyyy HH:mm:ss"),
                                                                                                                                                                                                                                                  Placa = obj.Pedagio.Veiculo != null ? obj.Pedagio.Veiculo.Placa : string.Empty,
                                                                                                                                                                                                                                                  CodigoVeiculo = obj.Pedagio.Veiculo != null ? obj.Pedagio.Veiculo.Codigo : 0,
                                                                                                                                                                                                                                                  obj.Pedagio.Praca,
                                                                                                                                                                                                                                                  obj.Pedagio.Rodovia,
                                                                                                                                                                                                                                                  Valor = obj.Pedagio.Valor.ToString("n2"),
                                                                                                                                                                                                                                                  SemParar = obj.Pedagio.ImportadoDeSemParar,
                                                                                                                                                                                                                                                  Tipo = obj.Pedagio.ImportadoDeSemParar == true ? "Sem Parar" : "Pago pelo Motorista",
                                                                                                                                                                                                                                                  PedagioDuplicado = repAcertoPedagio.PedagioDuplicado(obj.Pedagio.TipoPedagio, obj.Pedagio.Praca, obj.Pedagio.Rodovia, obj.Pedagio.Data, obj.Pedagio.Veiculo.Codigo),
                                                                                                                                                                                                                                                  obj.Pedagio.SituacaoPedagio,
                                                                                                                                                                                                                                                  DT_RowColor = "#FFFFFF",
                                                                                                                                                                                                                                                  obj.Pedagio.TipoPedagio,
                                                                                                                                                                                                                                                  obj.Pedagio.MoedaCotacaoBancoCentral,
                                                                                                                                                                                                                                                  DataBaseCRT = obj.Pedagio.DataBaseCRT.HasValue ? obj.Pedagio.DataBaseCRT.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                                                                                                                                                                                                                                                  ValorMoedaCotacao = obj.Pedagio.ValorMoedaCotacao.ToString("n4"),
                                                                                                                                                                                                                                                  ValorOriginalMoedaEstrangeira = obj.Pedagio.ValorOriginalMoedaEstrangeira.ToString("n2")
                                                                                                                                                                                                                                              }).ToList() : null,
                ListaPedagiosCredito = (etapa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapasAcertoViagem.Pedagios || etapa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapasAcertoViagem.Todas) && acertoViagem.Pedagios != null ? (from obj in acertoViagem.Pedagios
                                                                                                                                                                                                                                                     where obj.Pedagio.TipoPedagio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedagio.Credito
                                                                                                                                                                                                                                                     orderby obj.Pedagio.Data
                                                                                                                                                                                                                                                     select new
                                                                                                                                                                                                                                                     {
                                                                                                                                                                                                                                                         DataHoraPedagio = obj.Pedagio.Data,
                                                                                                                                                                                                                                                         CodigoAcertoPedagio = obj.Codigo,
                                                                                                                                                                                                                                                         obj.Pedagio.Codigo,
                                                                                                                                                                                                                                                         obj.LancadoManualmente,
                                                                                                                                                                                                                                                         DataHora = obj.Pedagio.Data.ToString("dd/MM/yyyy HH:mm:ss"),
                                                                                                                                                                                                                                                         Placa = obj.Pedagio.Veiculo != null ? obj.Pedagio.Veiculo.Placa : string.Empty,
                                                                                                                                                                                                                                                         CodigoVeiculo = obj.Pedagio.Veiculo != null ? obj.Pedagio.Veiculo.Codigo : 0,
                                                                                                                                                                                                                                                         obj.Pedagio.Praca,
                                                                                                                                                                                                                                                         obj.Pedagio.Rodovia,
                                                                                                                                                                                                                                                         Valor = obj.Pedagio.Valor.ToString("n2"),
                                                                                                                                                                                                                                                         SemParar = obj.Pedagio.ImportadoDeSemParar,
                                                                                                                                                                                                                                                         Tipo = "Crédito",
                                                                                                                                                                                                                                                         PedagioDuplicado = repAcertoPedagio.PedagioDuplicado(obj.Pedagio.TipoPedagio, obj.Pedagio.Praca, obj.Pedagio.Rodovia, obj.Pedagio.Data, obj.Pedagio.Veiculo.Codigo),
                                                                                                                                                                                                                                                         obj.Pedagio.SituacaoPedagio,
                                                                                                                                                                                                                                                         DT_RowColor = "#FFFFFF",
                                                                                                                                                                                                                                                         obj.Pedagio.TipoPedagio,
                                                                                                                                                                                                                                                         obj.Pedagio.MoedaCotacaoBancoCentral,
                                                                                                                                                                                                                                                         DataBaseCRT = obj.Pedagio.DataBaseCRT.HasValue ? obj.Pedagio.DataBaseCRT.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                                                                                                                                                                                                                                                         ValorMoedaCotacao = obj.Pedagio.ValorMoedaCotacao.ToString("n4"),
                                                                                                                                                                                                                                                         ValorOriginalMoedaEstrangeira = obj.Pedagio.ValorOriginalMoedaEstrangeira.ToString("n2")
                                                                                                                                                                                                                                                     }).ToList() : null,
                acertoViagem.Situacao,
                ListaVeiculos = acertoViagem.Veiculos != null ? (from obj in acertoViagem.Veiculos
                                                                 orderby obj.Veiculo.Placa
                                                                 select new
                                                                 {
                                                                     obj.Veiculo.Codigo,
                                                                     obj.Veiculo.Placa,
                                                                     obj.Veiculo.DescricaoMarca,
                                                                     obj.Veiculo.DescricaoModelo,
                                                                     obj.Veiculo.DescricaoTipoRodado,
                                                                     obj.Veiculo.DescricaoTipoVeiculo,
                                                                     obj.Veiculo.DescricaoTipoCarroceria,
                                                                     obj.Veiculo.DescricaoTipo,
                                                                     Equipamentos = obj.Veiculo.Equipamentos != null && obj.Veiculo.Equipamentos.Count > 0 ? "(" + string.Join(", ", obj.Veiculo.Equipamentos.Select(e => e.Descricao).ToList()) + ")" : ""
                                                                 }).ToList() : null,
                ListaVeiculosFechamento = acertoViagem.Veiculos != null ? (from obj in acertoViagem.Veiculos
                                                                           orderby obj.Veiculo.Placa
                                                                           select new
                                                                           {
                                                                               obj.Veiculo.Codigo,
                                                                               CodigoAcerto = obj.AcertoViagem.Codigo,
                                                                               obj.Veiculo.Placa,
                                                                               Reboque = string.Join(", ", obj.Veiculo.VeiculosVinculados.Select(o => o.Placa))
                                                                           }).ToList() : null,
                ListaInfracoes = acertoViagem.Infracoes != null ? (from obj in acertoViagem.Infracoes
                                                                   where obj.Infracao != null
                                                                   select new
                                                                   {
                                                                       obj.Codigo,
                                                                       CodigoAcerto = obj.AcertoViagem.Codigo,
                                                                       Placa = obj.Infracao?.Veiculo?.Placa ?? "",
                                                                       NumeroAtuacao = obj.Infracao?.NumeroAtuacao ?? "",
                                                                       NumeroInfracao = obj.Infracao?.Numero.ToString() ?? "",
                                                                       obj.InfracaoAssinada,
                                                                       DT_RowColor = "#FFFFFF"
                                                                   }).ToList() : null,
                ListaVeiculosArla = acertoViagem.Veiculos != null ? (from obj in acertoViagem.Veiculos
                                                                     where obj.Veiculo.Modelo != null ? obj.Veiculo.Modelo.PossuiArla32 == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SimNao.Sim : 1 == 1
                                                                     orderby obj.Veiculo.Placa
                                                                     select new
                                                                     {
                                                                         obj.Veiculo.Codigo,
                                                                         obj.Veiculo.Placa,
                                                                         obj.Veiculo.DescricaoMarca,
                                                                         obj.Veiculo.DescricaoModelo,
                                                                         obj.Veiculo.DescricaoTipoRodado,
                                                                         obj.Veiculo.DescricaoTipoVeiculo,
                                                                         obj.Veiculo.DescricaoTipoCarroceria,
                                                                         obj.Veiculo.DescricaoTipo
                                                                     }).ToList() : null,
                acertoViagem.DescricaoPeriodo,
                acertoViagem.DescricaoSituacao,
                NomeMotorista = acertoViagem.Motorista != null ? acertoViagem.Motorista.Nome : string.Empty,
                Placas = acertoViagem.Veiculos != null ? string.Join(", ", (from obj in acertoViagem.Veiculos select obj.Veiculo.Placa)) : string.Empty,
                TotalCargas = acertoViagem.Cargas != null ? acertoViagem.Cargas.Count() : 0,
                FreteTotal = acertoViagem.Cargas != null ? (from obj in acertoViagem.Cargas select obj.Carga.ValorFrete).Sum().ToString("n2") : 0.ToString("n2"),
            };

        }

        public void AtualizarCargasAcerto(Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem, Repositorio.UnitOfWork unidadeDeTrabalho, string listaCargas, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Servicos.Log.GravarInfo(acertoViagem?.Codigo.ToString() + " Inicio AtualizarCargasAcerto " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");

            Repositorio.Embarcador.Acerto.AcertoCarga repAcertoCarga = new Repositorio.Embarcador.Acerto.AcertoCarga(unidadeDeTrabalho);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeDeTrabalho);
            Repositorio.Embarcador.Acerto.AcertoCargaBonificacao repAcertoCargaBonificacao = new Repositorio.Embarcador.Acerto.AcertoCargaBonificacao(unidadeDeTrabalho);
            Repositorio.Embarcador.Acerto.AcertoCargaPedagio repAcertoCargaPedagio = new Repositorio.Embarcador.Acerto.AcertoCargaPedagio(unidadeDeTrabalho);


            List<int> listaAcertoCarga = repAcertoCarga.BuscarCargasDoAcerto(acertoViagem.Codigo);
            List<int> codigoCargas = new List<int>();

            dynamic listaGargas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(listaCargas);

            foreach (var carga in listaGargas)
            {
                codigoCargas.Add(int.Parse((string)carga.Carga.Codigo));

                if (!listaAcertoCarga.Contains(int.Parse((string)carga.Carga.Codigo)))
                {
                    Dominio.Entidades.Embarcador.Acerto.AcertoCarga cargaAcerto = new Dominio.Entidades.Embarcador.Acerto.AcertoCarga();
                    cargaAcerto.AcertoViagem = acertoViagem;
                    cargaAcerto.Carga = repCarga.BuscarPorCodigo(int.Parse((string)carga.Carga.Codigo));
                    cargaAcerto.PedagioAcerto = Utilidades.Decimal.Converter((string)carga.Carga.PedagioAcerto);
                    cargaAcerto.PedagioAcertoCredito = Utilidades.Decimal.Converter((string)carga.Carga.PedagioAcertoCredito);
                    cargaAcerto.PercentualAcerto = Utilidades.Decimal.Converter((string)carga.Carga.PercentualAcerto);
                    cargaAcerto.LancadoManualmente = bool.Parse((string)carga.Carga.LancadoManualmente);
                    cargaAcerto.ValorBonificacaoCliente = Utilidades.Decimal.Converter((string)carga.Carga.ValorBonificacaoCliente);
                    cargaAcerto.CargaFracionada = bool.Parse((string)carga.Carga.CargaFracionada);
                    cargaAcerto.ValorBrutoCarga = repCarga.BuscarValorFreteAReceberConhecimentos(cargaAcerto.Carga.Codigo);
                    cargaAcerto.ValorICMSCarga = repCarga.BuscarValorICMSConhecimentos(cargaAcerto.Carga.Codigo);

                    if (cargaAcerto.PercentualAcerto < 100)
                    {
                        if (cargaAcerto.ValorICMSCarga > 0)
                            cargaAcerto.ValorICMSCarga = cargaAcerto.ValorICMSCarga * (cargaAcerto.PercentualAcerto / 100);

                        if (cargaAcerto.ValorBrutoCarga > 0)
                            cargaAcerto.ValorBrutoCarga = cargaAcerto.ValorBrutoCarga * (cargaAcerto.PercentualAcerto / 100);
                    }
                    else if (cargaAcerto.CargaFracionada)
                    {
                        cargaAcerto.ValorICMSCarga = Utilidades.Decimal.Converter((string)carga.Carga.ValorICMSCarga);
                        cargaAcerto.ValorBrutoCarga = Utilidades.Decimal.Converter((string)carga.Carga.ValorBrutoCarga);
                    }

                    int codigoAcertoCarga = 0;
                    int.TryParse((string)carga.Carga.CodigoAcertoCarga, out codigoAcertoCarga);

                    if (codigoAcertoCarga == 0)
                    {
                        InserirPedagioDaNovaCarga(acertoViagem, cargaAcerto.Carga, unidadeDeTrabalho);
                        InserirAbastecimentoDaNovaCarga(acertoViagem, cargaAcerto.Carga, unidadeDeTrabalho, auditado);
                    }

                    repAcertoCarga.Inserir(cargaAcerto);

                    Servicos.Embarcador.Pedido.Pedido.AtualizarSituacaoPlanejamentoPedidoTMS(cargaAcerto.Carga, null, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPlanejamentoPedidoTMS.CargaPossuiAcertoAberto, unidadeDeTrabalho);
                }
                else
                {
                    Dominio.Entidades.Embarcador.Acerto.AcertoCarga cargaAcerto = repAcertoCarga.BuscarPorCodigoAcertoECarga(acertoViagem.Codigo, int.Parse((string)carga.Carga.Codigo));
                    if (cargaAcerto != null)
                    {
                        cargaAcerto.AcertoViagem = acertoViagem;
                        cargaAcerto.Carga = repCarga.BuscarPorCodigo(int.Parse((string)carga.Carga.Codigo));
                        cargaAcerto.PedagioAcerto = Utilidades.Decimal.Converter((string)carga.Carga.PedagioAcerto);
                        cargaAcerto.PedagioAcertoCredito = Utilidades.Decimal.Converter((string)carga.Carga.PedagioAcertoCredito);
                        cargaAcerto.PercentualAcerto = Utilidades.Decimal.Converter((string)carga.Carga.PercentualAcerto);
                        cargaAcerto.LancadoManualmente = bool.Parse((string)carga.Carga.LancadoManualmente);
                        cargaAcerto.ValorBonificacaoCliente = Utilidades.Decimal.Converter((string)carga.Carga.ValorBonificacaoCliente);
                        cargaAcerto.CargaFracionada = bool.Parse((string)carga.Carga.CargaFracionada);

                        cargaAcerto.ValorBrutoCarga = repCarga.BuscarValorFreteAReceberConhecimentos(cargaAcerto.Carga.Codigo);
                        cargaAcerto.ValorICMSCarga = repCarga.BuscarValorICMSConhecimentos(cargaAcerto.Carga.Codigo);
                        if (cargaAcerto.PercentualAcerto < 100)
                        {
                            if (cargaAcerto.ValorICMSCarga > 0)
                                cargaAcerto.ValorICMSCarga = cargaAcerto.ValorICMSCarga * (cargaAcerto.PercentualAcerto / 100);
                            if (cargaAcerto.ValorBrutoCarga > 0)
                                cargaAcerto.ValorBrutoCarga = cargaAcerto.ValorBrutoCarga * (cargaAcerto.PercentualAcerto / 100);

                        }
                        else if (cargaAcerto.CargaFracionada)
                        {
                            cargaAcerto.ValorICMSCarga = Utilidades.Decimal.Converter((string)carga.Carga.ValorICMSCarga);
                            cargaAcerto.ValorBrutoCarga = Utilidades.Decimal.Converter((string)carga.Carga.ValorBrutoCarga);
                        }

                        int codigoAcertoCarga = 0;
                        int.TryParse((string)carga.Carga.CodigoAcertoCarga, out codigoAcertoCarga);
                        if (codigoAcertoCarga == 0)
                        {
                            InserirPedagioDaNovaCarga(acertoViagem, cargaAcerto.Carga, unidadeDeTrabalho);
                            InserirAbastecimentoDaNovaCarga(acertoViagem, cargaAcerto.Carga, unidadeDeTrabalho, auditado);
                        }

                        repAcertoCarga.Atualizar(cargaAcerto);
                    }
                }
            }

            for (int i = 0; i < listaAcertoCarga.Count; i++)
            {
                if (!codigoCargas.Contains(listaAcertoCarga[i]))
                {
                    Dominio.Entidades.Embarcador.Acerto.AcertoCarga cargaAcerto = repAcertoCarga.BuscarPorCodigoAcertoECarga(acertoViagem.Codigo, listaAcertoCarga[i]);
                    List<Dominio.Entidades.Embarcador.Acerto.AcertoCargaBonificacao> listaBonificacao = repAcertoCargaBonificacao.BuscarPorAcertoCarga(cargaAcerto.Codigo);
                    for (int k = 0; k < listaBonificacao.Count(); k++)
                    {
                        repAcertoCargaBonificacao.Deletar(listaBonificacao[k]);
                    }
                    List<Dominio.Entidades.Embarcador.Acerto.AcertoCargaPedagio> listaPedagios = repAcertoCargaPedagio.BuscarPorAcertoCarga(cargaAcerto.Codigo);
                    for (int k = 0; k < listaPedagios.Count(); k++)
                    {
                        repAcertoCargaPedagio.Deletar(listaPedagios[k]);
                    }

                    repAcertoCarga.Deletar(cargaAcerto);
                }
            }

            Servicos.Log.GravarInfo(acertoViagem?.Codigo.ToString() + " Fim AtualizarCargasAcerto " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
        }

        public void AtualizarVeiculoAcerto(Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem, Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Servicos.Log.GravarInfo(acertoViagem?.Codigo.ToString() + " Inicio AtualizarVeiculoAcerto " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");

            Repositorio.Embarcador.Acerto.AcertoVeiculo repAcertoVeiculo = new Repositorio.Embarcador.Acerto.AcertoVeiculo(unidadeDeTrabalho);
            Repositorio.Embarcador.Acerto.AcertoOutraDespesa repAcertoOutraDespesa = new Repositorio.Embarcador.Acerto.AcertoOutraDespesa(unidadeDeTrabalho);
            Repositorio.Embarcador.Acerto.AcertoAbastecimento repAcertoAbastecimento = new Repositorio.Embarcador.Acerto.AcertoAbastecimento(unidadeDeTrabalho);
            Repositorio.Embarcador.Acerto.AcertoResumoAbastecimento repAcertoResumoAbastecimento = new Repositorio.Embarcador.Acerto.AcertoResumoAbastecimento(unidadeDeTrabalho);
            Repositorio.Embarcador.Acerto.AcertoPedagio repAcertoPedagio = new Repositorio.Embarcador.Acerto.AcertoPedagio(unidadeDeTrabalho);

            List<Dominio.Entidades.Veiculo> listaVeiculo = repAcertoVeiculo.BuscarVeiculosAcertoCarga(acertoViagem.Codigo);
            if (listaVeiculo == null || listaVeiculo.Count == 0)
                return;

            List<Dominio.Entidades.Embarcador.Acerto.AcertoVeiculo> listaAcertoVeiculo = repAcertoVeiculo.BuscarPorCodigoAcerto(acertoViagem.Codigo);
            for (int i = 0; i < listaAcertoVeiculo.Count(); i++)
                repAcertoVeiculo.Deletar(listaAcertoVeiculo[i]);

            for (int i = 0; i < listaVeiculo.Count(); i++)
            {
                if (listaVeiculo[i] == null)
                    continue;
                if (!repAcertoVeiculo.VeiculoEmAcerto(acertoViagem, listaVeiculo[i]))
                {
                    Dominio.Entidades.Embarcador.Acerto.AcertoVeiculo acertoVeiculo = new Dominio.Entidades.Embarcador.Acerto.AcertoVeiculo();
                    acertoVeiculo.AcertoViagem = acertoViagem;
                    acertoVeiculo.Veiculo = listaVeiculo[i];

                    repAcertoVeiculo.Inserir(acertoVeiculo);
                }
            }
            listaVeiculo = repAcertoVeiculo.BuscarVeiculosVinculadosAcertoCarga(acertoViagem.Codigo);
            for (int i = 0; i < listaVeiculo.Count(); i++)
            {
                if (listaVeiculo[i] == null)
                    continue;
                if (!repAcertoVeiculo.VeiculoEmAcerto(acertoViagem, listaVeiculo[i]))
                {
                    Dominio.Entidades.Embarcador.Acerto.AcertoVeiculo acertoVeiculo = new Dominio.Entidades.Embarcador.Acerto.AcertoVeiculo();
                    acertoVeiculo.AcertoViagem = acertoViagem;
                    acertoVeiculo.Veiculo = listaVeiculo[i];

                    repAcertoVeiculo.Inserir(acertoVeiculo);
                }
            }

            RemoverPedagiosSemCarga(acertoViagem, unidadeDeTrabalho);
            RemoverAbastecimentosSemCarga(acertoViagem, unidadeDeTrabalho);
            RemoverOutrasDepesasSemCarga(acertoViagem, unidadeDeTrabalho, auditado);
            AtualizarVeiculosSegmento(acertoViagem, unidadeDeTrabalho);

            Servicos.Log.GravarInfo(acertoViagem?.Codigo.ToString() + " Fim AtualizarVeiculoAcerto " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
        }

        public void AtualizarPedagiosAcerto(Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem, Repositorio.UnitOfWork unidadeDeTrabalho, string listaRequestPedagios, string listaRequestPedagiosCredito, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Servicos.Log.GravarInfo(acertoViagem?.Codigo.ToString() + " Inicio AtualizarPedagiosAcerto " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");

            Repositorio.Embarcador.Acerto.AcertoPedagio repAcertoPedagio = new Repositorio.Embarcador.Acerto.AcertoPedagio(unidadeDeTrabalho);
            Repositorio.Embarcador.Pedagio.Pedagio repPedagio = new Repositorio.Embarcador.Pedagio.Pedagio(unidadeDeTrabalho);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoAcertoViagem repConfiguracaoFinanceiraContratoAcertoViagem = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoAcertoViagem(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraContratoAcertoViagem configuracaoAcertoViagem = repConfiguracaoFinanceiraContratoAcertoViagem.BuscarPrimeiroRegistro();
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeDeTrabalho);

            repAcertoPedagio.DeletarPorAcerto(acertoViagem.Codigo);

            dynamic listaPedagios = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(listaRequestPedagios);

            foreach (var pedagio in listaPedagios)
            {
                Dominio.Entidades.Embarcador.Acerto.AcertoPedagio acertoPedagio = new Dominio.Entidades.Embarcador.Acerto.AcertoPedagio();

                if (int.Parse((string)pedagio.Pedagio.Codigo) < 0 || int.Parse((string)pedagio.Pedagio.Codigo) == 0)
                {
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedagio tipoPedagio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedagio.Debito;
                    Enum.TryParse((string)pedagio.Pedagio.TipoPedagio, out tipoPedagio);

                    Dominio.Entidades.Embarcador.Pedagio.Pedagio pedagioNovo = new Dominio.Entidades.Embarcador.Pedagio.Pedagio();
                    pedagioNovo.Data = DateTime.Parse((string)pedagio.Pedagio.DataHora);
                    pedagioNovo.ImportadoDeSemParar = false;
                    pedagioNovo.Praca = (string)pedagio.Pedagio.Praca;
                    pedagioNovo.Rodovia = (string)pedagio.Pedagio.Rodovia;
                    pedagioNovo.Valor = Utilidades.Decimal.Converter((string)pedagio.Pedagio.Valor);
                    pedagioNovo.Veiculo = repVeiculo.BuscarPorCodigo(int.Parse((string)pedagio.Pedagio.CodigoVeiculo));
                    pedagioNovo.SituacaoPedagio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedagio.Fechado;
                    pedagioNovo.DataAlteracao = DateTime.Now;
                    pedagioNovo.TipoPedagio = tipoPedagio;

                    pedagioNovo.MoedaCotacaoBancoCentral = ((string)pedagio.Pedagio.MoedaCotacaoBancoCentral).ToNullableEnum<Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral>();
                    pedagioNovo.DataBaseCRT = ((string)pedagio.Pedagio.DataBaseCRT).ToNullableDateTime();
                    pedagioNovo.ValorMoedaCotacao = Utilidades.Decimal.Converter((string)pedagio.Pedagio.ValorMoedaCotacao);
                    pedagioNovo.ValorOriginalMoedaEstrangeira = Utilidades.Decimal.Converter((string)pedagio.Pedagio.ValorOriginalMoedaEstrangeira);

                    if (configuracaoAcertoViagem != null && configuracaoAcertoViagem.GerarMovimentoAutomaticoNaGeracaoAcertoViagem && configuracaoAcertoViagem.TipoMovimentoPedagioPagoPeloMotorista != null)
                        pedagioNovo.TipoMovimento = configuracaoAcertoViagem.TipoMovimentoPedagioPagoPeloMotorista;

                    repPedagio.Inserir(pedagioNovo);

                    acertoPedagio.Pedagio = pedagioNovo;
                }
                else
                    acertoPedagio.Pedagio = repPedagio.BuscarPorCodigo(int.Parse((string)pedagio.Pedagio.Codigo));

                acertoPedagio.AcertoViagem = acertoViagem;
                acertoPedagio.LancadoManualmente = bool.Parse((string)pedagio.Pedagio.LancadoManualmente);
                repAcertoPedagio.Inserir(acertoPedagio);
            }

            dynamic listaPedagiosCredito = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(listaRequestPedagiosCredito);

            if (listaPedagiosCredito != null)
            {
                foreach (var pedagio in listaPedagiosCredito)
                {
                    Dominio.Entidades.Embarcador.Acerto.AcertoPedagio acertoPedagio = new Dominio.Entidades.Embarcador.Acerto.AcertoPedagio();

                    if (int.Parse((string)pedagio.Pedagio.Codigo) < 0 || int.Parse((string)pedagio.Pedagio.Codigo) == 0)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedagio tipoPedagio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedagio.Credito;
                        Enum.TryParse((string)pedagio.Pedagio.TipoPedagio, out tipoPedagio);

                        Dominio.Entidades.Embarcador.Pedagio.Pedagio pedagioNovo = new Dominio.Entidades.Embarcador.Pedagio.Pedagio();
                        pedagioNovo.Data = DateTime.Parse((string)pedagio.Pedagio.DataHora);
                        pedagioNovo.ImportadoDeSemParar = false;
                        pedagioNovo.Praca = (string)pedagio.Pedagio.Praca;
                        pedagioNovo.Rodovia = (string)pedagio.Pedagio.Rodovia;
                        pedagioNovo.Valor = Utilidades.Decimal.Converter((string)pedagio.Pedagio.Valor);
                        pedagioNovo.Veiculo = repVeiculo.BuscarPorCodigo(int.Parse((string)pedagio.Pedagio.CodigoVeiculo));
                        pedagioNovo.SituacaoPedagio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedagio.Fechado;
                        pedagioNovo.DataAlteracao = DateTime.Now;
                        pedagioNovo.TipoPedagio = tipoPedagio;
                        pedagioNovo.TipoMovimento = null;

                        pedagioNovo.MoedaCotacaoBancoCentral = ((string)pedagio.Pedagio.MoedaCotacaoBancoCentral).ToNullableEnum<Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral>();
                        pedagioNovo.DataBaseCRT = ((string)pedagio.Pedagio.DataBaseCRT).ToNullableDateTime();
                        pedagioNovo.ValorMoedaCotacao = Utilidades.Decimal.Converter((string)pedagio.Pedagio.ValorMoedaCotacao);
                        pedagioNovo.ValorOriginalMoedaEstrangeira = Utilidades.Decimal.Converter((string)pedagio.Pedagio.ValorOriginalMoedaEstrangeira);

                        repPedagio.Inserir(pedagioNovo);

                        acertoPedagio.Pedagio = pedagioNovo;
                    }
                    else
                        acertoPedagio.Pedagio = repPedagio.BuscarPorCodigo(int.Parse((string)pedagio.Pedagio.Codigo));

                    acertoPedagio.AcertoViagem = acertoViagem;
                    acertoPedagio.LancadoManualmente = bool.Parse((string)pedagio.Pedagio.LancadoManualmente);
                    repAcertoPedagio.Inserir(acertoPedagio);
                }
            }

            RemoverPedagiosSemCarga(acertoViagem, unidadeDeTrabalho);
            RemoverAbastecimentosSemCarga(acertoViagem, unidadeDeTrabalho);
            RemoverOutrasDepesasSemCarga(acertoViagem, unidadeDeTrabalho, auditado);

            Servicos.Log.GravarInfo(acertoViagem?.Codigo.ToString() + " Fim AtualizarPedagiosAcerto " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
        }

        public void InserirPedagioDaNovaCarga(Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem, Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Servicos.Log.GravarInfo(acertoViagem?.Codigo.ToString() + " Inicio InserirPedagioDaNovaCarga " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");

            if (acertoViagem.Pedagios != null && acertoViagem.Pedagios.Count > 0)
            {
                Servicos.Log.GravarInfo(acertoViagem?.Codigo.ToString() + " Fim InserirPedagioDaNovaCarga " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                return;
            }

            if (carga.Veiculo == null)
                return;

            Repositorio.Embarcador.Acerto.AcertoPedagio repAcertoPedagio = new Repositorio.Embarcador.Acerto.AcertoPedagio(unidadeDeTrabalho);
            Repositorio.Embarcador.Pedagio.Pedagio repPedagio = new Repositorio.Embarcador.Pedagio.Pedagio(unidadeDeTrabalho);
            Repositorio.Embarcador.Cargas.CargaRegistroEncerramento repCargaRegistroEncerramento = new Repositorio.Embarcador.Cargas.CargaRegistroEncerramento(unidadeDeTrabalho);

            Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramento cargaRegistroEncerramento = repCargaRegistroEncerramento.BuscarPorCarga(carga.Codigo);

            List<Dominio.Entidades.Embarcador.Pedagio.Pedagio> listaPedagio = repPedagio.BuscarPedagioPorVeiculoDataSemAcerto(carga.Veiculo.Codigo, acertoViagem.DataInicial, cargaRegistroEncerramento != null && cargaRegistroEncerramento.DataEncerramento.HasValue && cargaRegistroEncerramento.DataEncerramento.Value > DateTime.MinValue ? cargaRegistroEncerramento.DataEncerramento.Value : acertoViagem.DataFinal.HasValue ? acertoViagem.DataFinal.Value : DateTime.Now.Date);

            int countPedagios = listaPedagio.Count;

            for (int i = 0; i < countPedagios; i++)
            {
                Dominio.Entidades.Embarcador.Acerto.AcertoPedagio acertoPedagio = new Dominio.Entidades.Embarcador.Acerto.AcertoPedagio();
                acertoPedagio.AcertoViagem = acertoViagem;
                acertoPedagio.Pedagio = listaPedagio[i];
                acertoPedagio.LancadoManualmente = false;

                repAcertoPedagio.Inserir(acertoPedagio);
            }

            Servicos.Log.GravarInfo(acertoViagem?.Codigo.ToString() + " Fim InserirPedagioDaNovaCarga " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
        }

        public void RemoverPedagiosSemCarga(Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Servicos.Log.GravarInfo(acertoViagem?.Codigo.ToString() + " Inicio RemoverPedagiosSemCarga " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");

            Repositorio.Embarcador.Acerto.AcertoVeiculo repAcertoVeiculo = new Repositorio.Embarcador.Acerto.AcertoVeiculo(unidadeDeTrabalho);
            Repositorio.Embarcador.Acerto.AcertoPedagio repAcertoPedagio = new Repositorio.Embarcador.Acerto.AcertoPedagio(unidadeDeTrabalho);
            List<Dominio.Entidades.Embarcador.Acerto.AcertoPedagio> listaAcertoPedagio = repAcertoPedagio.BuscarPorCodigoAcerto(acertoViagem.Codigo);

            int count = listaAcertoPedagio.Count;

            for (int i = 0; i < count; i++)
            {
                Dominio.Entidades.Embarcador.Acerto.AcertoPedagio acertoPedagio = listaAcertoPedagio[i];

                if (!repAcertoVeiculo.VeiculoEmAcerto(acertoViagem, acertoPedagio.Pedagio.Veiculo))
                {
                    repAcertoPedagio.Deletar(acertoPedagio);
                }
            }

            Servicos.Log.GravarInfo(acertoViagem?.Codigo.ToString() + " Fim RemoverPedagiosSemCarga " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
        }

        public void InserirAbastecimentoDaNovaCarga(Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem, Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Servicos.Log.GravarInfo(acertoViagem?.Codigo.ToString() + " Inicio InserirAbastecimentoDaNovaCarga " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");

            if (carga.Veiculo == null)
                return;

            Repositorio.Embarcador.Acerto.AcertoAbastecimento repAcertoAbastecimento = new Repositorio.Embarcador.Acerto.AcertoAbastecimento(unidadeDeTrabalho);
            Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(unidadeDeTrabalho);
            Repositorio.Embarcador.Cargas.CargaRegistroEncerramento repCargaRegistroEncerramento = new Repositorio.Embarcador.Cargas.CargaRegistroEncerramento(unidadeDeTrabalho);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeDeTrabalho);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Cargas.CargaRegistroEncerramento cargaRegistroEncerramento = null;
            if (!configuracaoTMS.UtilizaMoedaEstrangeira)
                cargaRegistroEncerramento = repCargaRegistroEncerramento.BuscarPorCarga(carga.Codigo);

            List<Dominio.Entidades.Abastecimento> listaAbastecimento = repAbastecimento.BuscarAbastecimentoPorVeiculoDataSemAcerto(carga.Veiculo.Codigo, acertoViagem.DataInicial, cargaRegistroEncerramento != null && cargaRegistroEncerramento.DataEncerramento.HasValue && cargaRegistroEncerramento.DataEncerramento.Value > DateTime.MinValue ? cargaRegistroEncerramento.DataEncerramento.Value : acertoViagem.DataFinal.HasValue ? acertoViagem.DataFinal.Value : DateTime.Now.Date);

            int countAbastecimentos = listaAbastecimento.Count;

            for (int i = 0; i < countAbastecimentos; i++)
            {
                Dominio.Entidades.Embarcador.Acerto.AcertoAbastecimento acertoAbastecimento = new Dominio.Entidades.Embarcador.Acerto.AcertoAbastecimento
                {
                    AcertoViagem = acertoViagem,
                    Abastecimento = listaAbastecimento[i],
                    LancadoManualmente = false
                };

                repAcertoAbastecimento.Inserir(acertoAbastecimento);

                if (acertoViagem.Motorista != null)
                {
                    Dominio.Entidades.Abastecimento abastecimento = repAbastecimento.BuscarPorCodigo(listaAbastecimento[i].Codigo);
                    abastecimento.Motorista = acertoViagem.Motorista;
                    abastecimento.Integrado = false;

                    repAbastecimento.Atualizar(abastecimento);
                    Servicos.Auditoria.Auditoria.Auditar(auditado, abastecimento, null, "Atualizou o motorista via Acerto de Viagem.", unidadeDeTrabalho);
                }

                //LancarResumoAbastecimento(acertoAbastecimento, unidadeDeTrabalho);
            }

            LancarResumoAbastecimento(acertoViagem, carga.Veiculo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Combustivel, unidadeDeTrabalho);
            LancarResumoAbastecimento(acertoViagem, carga.Veiculo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Arla, unidadeDeTrabalho);

            Servicos.Log.GravarInfo(acertoViagem?.Codigo.ToString() + " Fim InserirAbastecimentoDaNovaCarga " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
        }

        public void RemoverOutrasDepesasSemCarga(Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem, Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            if (acertoViagem.Cargas == null || acertoViagem.Cargas.Count == 0)
                return;
            if (acertoViagem.Veiculos == null || acertoViagem.Veiculos.Count == 0)
                return;
            Dominio.Entidades.Veiculo veiculo = acertoViagem.Veiculos.FirstOrDefault()?.Veiculo ?? null;
            if (veiculo == null)
                return;

            Servicos.Log.GravarInfo(acertoViagem?.Codigo.ToString() + " Inicio RemoverOutrasDepesasSemCarga " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");

            Repositorio.Embarcador.Acerto.AcertoVeiculo repAcertoVeiculo = new Repositorio.Embarcador.Acerto.AcertoVeiculo(unidadeDeTrabalho);
            Repositorio.Embarcador.Acerto.AcertoOutraDespesa repAcertoOutraDespesa = new Repositorio.Embarcador.Acerto.AcertoOutraDespesa(unidadeDeTrabalho);
            List<Dominio.Entidades.Embarcador.Acerto.AcertoOutraDespesa> listaAcertoOutraDespesa = repAcertoOutraDespesa.BuscarPorAcerto(acertoViagem.Codigo);

            for (int i = 0; i < listaAcertoOutraDespesa.Count(); i++)
            {
                if (!repAcertoVeiculo.VeiculoEmAcerto(acertoViagem, listaAcertoOutraDespesa[i].Veiculo))
                {
                    Servicos.Auditoria.Auditoria.Auditar(auditado, acertoViagem, null, "Atualizado despesa " + listaAcertoOutraDespesa[i].Descricao + " do veículo " + listaAcertoOutraDespesa[i].Veiculo.Placa + " para " + veiculo.Placa, unidadeDeTrabalho);
                    listaAcertoOutraDespesa[i].Veiculo = veiculo;
                    repAcertoOutraDespesa.Atualizar(listaAcertoOutraDespesa[i]);
                }
            }

            Servicos.Log.GravarInfo(acertoViagem?.Codigo.ToString() + " Fim RemoverOutrasDepesasSemCarga " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
        }

        public void RemoverAbastecimentosSemCarga(Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Servicos.Log.GravarInfo(acertoViagem?.Codigo.ToString() + " Inicio RemoverAbastecimentosSemCarga " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");

            Repositorio.Embarcador.Acerto.AcertoVeiculo repAcertoVeiculo = new Repositorio.Embarcador.Acerto.AcertoVeiculo(unidadeDeTrabalho);
            Repositorio.Embarcador.Acerto.AcertoAbastecimento repAcertoAbastecimento = new Repositorio.Embarcador.Acerto.AcertoAbastecimento(unidadeDeTrabalho);
            Repositorio.Embarcador.Acerto.AcertoResumoAbastecimento repAcertoResumoAbastecimento = new Repositorio.Embarcador.Acerto.AcertoResumoAbastecimento(unidadeDeTrabalho);
            List<Dominio.Entidades.Embarcador.Acerto.AcertoAbastecimento> listaAcertoAbastecimento = repAcertoAbastecimento.BuscarPorCodigoAcerto(acertoViagem.Codigo);

            for (int i = 0; i < listaAcertoAbastecimento.Count(); i++)
            {
                if (!repAcertoVeiculo.VeiculoEmAcerto(acertoViagem, listaAcertoAbastecimento[i].Abastecimento.Veiculo))
                {
                    RemoverResumoAbastecimento(listaAcertoAbastecimento[i], unidadeDeTrabalho);
                    repAcertoAbastecimento.Deletar(listaAcertoAbastecimento[i]);
                }
            }
            repAcertoResumoAbastecimento.DeletarResumoSemVeiculo(acertoViagem.Codigo);

            Servicos.Log.GravarInfo(acertoViagem?.Codigo.ToString() + " Fim RemoverAbastecimentosSemCarga " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
        }

        public void RemoverResumoAbastecimento(Dominio.Entidades.Embarcador.Acerto.AcertoAbastecimento acertoAbastecimento, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Servicos.Log.GravarInfo(acertoAbastecimento?.AcertoViagem?.Codigo.ToString() + " Inicio RemoverResumoAbastecimento " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");

            Repositorio.Embarcador.Acerto.AcertoAbastecimento repAcertoAbastecimento = new Repositorio.Embarcador.Acerto.AcertoAbastecimento(unidadeDeTrabalho);
            Repositorio.Embarcador.Acerto.AcertoResumoAbastecimento repAcertoResumoAbastecimento = new Repositorio.Embarcador.Acerto.AcertoResumoAbastecimento(unidadeDeTrabalho);

            Dominio.Entidades.Embarcador.Acerto.AcertoResumoAbastecimento resumoAbastecimento;

            resumoAbastecimento = repAcertoResumoAbastecimento.BuscarPorCodigoAcertoVeiculoTipo(acertoAbastecimento.AcertoViagem.Codigo, acertoAbastecimento.Abastecimento.Veiculo.Codigo, acertoAbastecimento.Abastecimento.TipoAbastecimento);
            if (resumoAbastecimento != null)
                repAcertoResumoAbastecimento.Deletar(resumoAbastecimento);

            Servicos.Log.GravarInfo(acertoAbastecimento?.AcertoViagem?.Codigo.ToString() + " Fim RemoverResumoAbastecimento " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
        }

        public void LancarResumoAbastecimento(Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem, Dominio.Entidades.Veiculo veiculo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento tipoAbastecimento, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Servicos.Log.GravarInfo(acertoViagem?.Codigo.ToString() + " Inicio LancarResumoAbastecimento " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");

            Repositorio.Embarcador.Acerto.AcertoAbastecimento repAcertoAbastecimento = new Repositorio.Embarcador.Acerto.AcertoAbastecimento(unidadeDeTrabalho);
            Repositorio.Embarcador.Acerto.AcertoResumoAbastecimento repAcertoResumoAbastecimento = new Repositorio.Embarcador.Acerto.AcertoResumoAbastecimento(unidadeDeTrabalho);

            Dominio.Entidades.Embarcador.Acerto.AcertoResumoAbastecimento resumoAbastecimento = repAcertoResumoAbastecimento.BuscarPorCodigoAcertoVeiculoTipo(acertoViagem.Codigo, veiculo.Codigo, tipoAbastecimento);

            if (!repAcertoAbastecimento.ContemAbastecimentoLancadoVeiculo(veiculo.Codigo, tipoAbastecimento))
            {
                Servicos.Log.GravarInfo(acertoViagem?.Codigo.ToString() + " Fim LancarResumoAbastecimento " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                return;
            }

            bool inserir = resumoAbastecimento == null;

            if (inserir)
                resumoAbastecimento = new Dominio.Entidades.Embarcador.Acerto.AcertoResumoAbastecimento();

            resumoAbastecimento.AcertoViagem = acertoViagem;
            resumoAbastecimento.Veiculo = veiculo;

            if (acertoViagem.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem.EmAntamento)
            {
                resumoAbastecimento.KMInicial = (int)repAcertoAbastecimento.KMInicialAbastecimentos(acertoViagem.Codigo, veiculo, tipoAbastecimento);
                resumoAbastecimento.HorimetroInicial = (int)repAcertoAbastecimento.HorimetroInicialAbastecimentos(acertoViagem.Codigo, veiculo, tipoAbastecimento);
            }
            resumoAbastecimento.KMFinal = (int)repAcertoAbastecimento.KMFinalAbastecimentos(acertoViagem.Codigo, veiculo.Codigo, tipoAbastecimento);
            resumoAbastecimento.HorimetroFinal = (int)repAcertoAbastecimento.HorimetroFinalAbastecimentos(acertoViagem.Codigo, veiculo.Codigo, tipoAbastecimento);
            resumoAbastecimento.Litros = repAcertoAbastecimento.QuantidadeLitrosAbastecimentos(acertoViagem.Codigo, veiculo.Codigo, tipoAbastecimento);
            resumoAbastecimento.ValorTotal = repAcertoAbastecimento.ValorTotalAbastecimentos(acertoViagem.Codigo, veiculo.Codigo, tipoAbastecimento);

            resumoAbastecimento.LitrosEquipamento = repAcertoAbastecimento.QuantidadeLitrosAbastecimentosEquipamento(acertoViagem.Codigo, veiculo.Codigo, tipoAbastecimento);
            resumoAbastecimento.ValorTotalEquipamento = repAcertoAbastecimento.ValorTotalAbastecimentosEquipamento(acertoViagem.Codigo, veiculo.Codigo, tipoAbastecimento);

            resumoAbastecimento.TipoAbastecimento = tipoAbastecimento;

            int horimetroRodado = resumoAbastecimento.HorimetroFinal - resumoAbastecimento.HorimetroInicial;
            if (horimetroRodado > 0)
            {
                if (horimetroRodado > 0 && resumoAbastecimento.LitrosEquipamento > 0)
                    resumoAbastecimento.MediaHorimetro = horimetroRodado / resumoAbastecimento.LitrosEquipamento;
                else
                    resumoAbastecimento.MediaHorimetro = 0;
                if (horimetroRodado < 0)
                    horimetroRodado = 0;
                resumoAbastecimento.HorimetroTotal = horimetroRodado;
                resumoAbastecimento.HorimetroTotalAjustado = horimetroRodado;
                resumoAbastecimento.PercentualAjusteHorimetro = 0;
            }

            int kmRodado = resumoAbastecimento.KMFinal - resumoAbastecimento.KMInicial;
            if (kmRodado > 0 && resumoAbastecimento.Litros > 0)
                resumoAbastecimento.Media = kmRodado / resumoAbastecimento.Litros;
            else
                resumoAbastecimento.Media = 0;
            if (kmRodado < 0)
                kmRodado = 0;
            resumoAbastecimento.KMTotal = kmRodado;
            resumoAbastecimento.KMTotalAjustado = kmRodado;
            resumoAbastecimento.PercentualAjuste = 0;

            if (resumoAbastecimento.TipoAbastecimento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Combustivel)
            {
                if (resumoAbastecimento.Media > 0 && resumoAbastecimento.MediaIdeal > 0 && !resumoAbastecimento.ResumoAprovado)
                    resumoAbastecimento.ResumoAprovado = resumoAbastecimento.Media >= resumoAbastecimento.MediaIdeal;
                if (!resumoAbastecimento.ResumoAprovado && resumoAbastecimento.MediaHorimetro > 0 && resumoAbastecimento.MediaIdealHorimetro > 0)
                    resumoAbastecimento.ResumoAprovado = resumoAbastecimento.MediaHorimetro >= resumoAbastecimento.MediaIdealHorimetro;
            }
            else
            {
                if (resumoAbastecimento.MediaIdeal > 0 && !resumoAbastecimento.ResumoAprovado)
                    resumoAbastecimento.ResumoAprovado = resumoAbastecimento.MediaIdeal > 0;
            }

            if (inserir)
                repAcertoResumoAbastecimento.Inserir(resumoAbastecimento);
            else
                repAcertoResumoAbastecimento.Atualizar(resumoAbastecimento);

            Servicos.Log.GravarInfo(acertoViagem?.Codigo.ToString() + " Fim LancarResumoAbastecimento " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
        }

        public List<string> PlacasAbastecimentoPendenteAutorizacao(int codigoAcerto, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento tipoAbastecimento, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            List<string> retorno = new List<string>();
            Servicos.Log.GravarInfo(codigoAcerto.ToString() + " Inicio ContemAbastecimentoPendenteAutorizacao " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");

            Repositorio.Embarcador.Acerto.AcertoResumoAbastecimento repAcertoResumoAbastecimento = new Repositorio.Embarcador.Acerto.AcertoResumoAbastecimento(unidadeDeTrabalho);
            Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(unidadeDeTrabalho);
            Repositorio.Embarcador.Acerto.AcertoAbastecimento repAcertoAbastecimento = new Repositorio.Embarcador.Acerto.AcertoAbastecimento(unidadeDeTrabalho);

            bool contemResumoPendente = false;
            List<Dominio.Entidades.Embarcador.Acerto.AcertoResumoAbastecimento> listaResumo = repAcertoResumoAbastecimento.BuscarPorCodigoAcertoTipo(codigoAcerto, tipoAbastecimento);
            if (listaResumo == null || listaResumo.Count() == 0)
            {
                Servicos.Log.GravarInfo(codigoAcerto.ToString() + " Fim ContemAbastecimentoPendenteAutorizacao " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                return retorno;
            }

            for (int i = 0; i < listaResumo.Count(); i++)
            {
                if (tipoAbastecimento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Arla)
                {
                    if (listaResumo[i].ResumoAprovado)
                        contemResumoPendente = false;
                    else if (listaResumo[i].Media > 0)
                        contemResumoPendente = listaResumo[i].MediaIdeal <= 0;
                    if (contemResumoPendente)
                    {
                        retorno.Add(listaResumo[i].Veiculo?.Placa ?? "");
                        break;
                    }
                }
                else
                {
                    if (listaResumo[i].Media > 0 && listaResumo[i].MediaIdeal > 0)
                    {
                        contemResumoPendente = (listaResumo[i].Media < listaResumo[i].MediaIdeal) && !listaResumo[i].ResumoAprovado;
                        if (contemResumoPendente)
                        {
                            retorno.Add(listaResumo[i].Veiculo?.Placa ?? "");
                            break;
                        }
                    }
                    else if (listaResumo[i].MediaIdeal <= 0 && listaResumo[i].Media > 0 && !listaResumo[i].ResumoAprovado)
                    {
                        contemResumoPendente = true;
                        retorno.Add(listaResumo[i].Veiculo?.Placa ?? "");
                        break;
                    }
                }

            }

            if (!contemResumoPendente)
            {
                List<Dominio.Entidades.Embarcador.Acerto.AcertoAbastecimento> listaAbastecimento = repAcertoAbastecimento.BuscarPorCodigoAcertoTipo(codigoAcerto, tipoAbastecimento);
                for (int i = 0; i < listaAbastecimento.Count(); i++)
                {
                    if (listaAbastecimento[i].Abastecimento.Posto != null && listaAbastecimento[i].Abastecimento.Posto.CPF_CNPJ_Formatado != listaAbastecimento[i].AcertoViagem.Motorista.CPF_Formatado)
                    {
                        contemResumoPendente = listaAbastecimento[i].Abastecimento.Situacao != "F";
                        if (contemResumoPendente)
                        {
                            retorno.Add(listaAbastecimento[i].Abastecimento?.Veiculo?.Placa ?? "");
                            break;
                        }

                        contemResumoPendente = repAbastecimento.AbastecimentoDuplicado(listaAbastecimento[i].Abastecimento);
                        if (contemResumoPendente)
                        {
                            contemResumoPendente = !repAcertoResumoAbastecimento.ResumoAutorizado(codigoAcerto, listaAbastecimento[i].Abastecimento.Veiculo.Codigo, tipoAbastecimento);
                            if (contemResumoPendente)
                            {
                                retorno.Add(listaAbastecimento[i].Abastecimento?.Veiculo?.Placa ?? "");
                                break;
                            }
                        }
                    }
                }
            }

            Servicos.Log.GravarInfo(codigoAcerto.ToString() + " Fim ContemAbastecimentoPendenteAutorizacao " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            if (retorno != null && retorno.Count > 0)
                retorno = retorno.Distinct().ToList();
            return retorno;
        }

        public bool ContemAbastecimentoPendenteAutorizacao(int codigoAcerto, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento tipoAbastecimento, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Servicos.Log.GravarInfo(codigoAcerto.ToString() + " Inicio ContemAbastecimentoPendenteAutorizacao " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");

            Repositorio.Embarcador.Acerto.AcertoResumoAbastecimento repAcertoResumoAbastecimento = new Repositorio.Embarcador.Acerto.AcertoResumoAbastecimento(unidadeDeTrabalho);
            Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(unidadeDeTrabalho);
            Repositorio.Embarcador.Acerto.AcertoAbastecimento repAcertoAbastecimento = new Repositorio.Embarcador.Acerto.AcertoAbastecimento(unidadeDeTrabalho);

            bool contemResumoPendente = false;
            List<Dominio.Entidades.Embarcador.Acerto.AcertoResumoAbastecimento> listaResumo = repAcertoResumoAbastecimento.BuscarPorCodigoAcertoTipo(codigoAcerto, tipoAbastecimento);
            if (listaResumo == null || listaResumo.Count() == 0)
            {
                Servicos.Log.GravarInfo(codigoAcerto.ToString() + " Fim ContemAbastecimentoPendenteAutorizacao " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                return false;
            }

            for (int i = 0; i < listaResumo.Count(); i++)
            {
                if (tipoAbastecimento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Arla)
                {
                    if (listaResumo[i].ResumoAprovado)
                        contemResumoPendente = false;
                    else if (listaResumo[i].Media > 0)
                        contemResumoPendente = listaResumo[i].MediaIdeal <= 0;
                    if (contemResumoPendente)
                        break;
                }
                else
                {
                    if (listaResumo[i].Media > 0 && listaResumo[i].MediaIdeal > 0)
                    {
                        contemResumoPendente = (listaResumo[i].Media < listaResumo[i].MediaIdeal) && !listaResumo[i].ResumoAprovado;
                        if (contemResumoPendente)
                            break;
                    }
                    else if (listaResumo[i].MediaIdeal <= 0 && listaResumo[i].Media > 0 && !listaResumo[i].ResumoAprovado)
                    {
                        contemResumoPendente = true;
                        break;
                    }
                }

            }

            if (!contemResumoPendente)
            {
                List<Dominio.Entidades.Embarcador.Acerto.AcertoAbastecimento> listaAbastecimento = repAcertoAbastecimento.BuscarPorCodigoAcertoTipo(codigoAcerto, tipoAbastecimento);
                for (int i = 0; i < listaAbastecimento.Count(); i++)
                {
                    if (listaAbastecimento[i].Abastecimento.Posto != null && listaAbastecimento[i].Abastecimento.Posto.CPF_CNPJ_Formatado != listaAbastecimento[i].AcertoViagem.Motorista.CPF_Formatado)
                    {
                        contemResumoPendente = listaAbastecimento[i].Abastecimento.Situacao != "F";
                        if (contemResumoPendente)
                            break;

                        contemResumoPendente = repAbastecimento.AbastecimentoDuplicado(listaAbastecimento[i].Abastecimento);
                        if (contemResumoPendente)
                        {
                            contemResumoPendente = !repAcertoResumoAbastecimento.ResumoAutorizado(codigoAcerto, listaAbastecimento[i].Abastecimento.Veiculo.Codigo, tipoAbastecimento);
                            if (contemResumoPendente)
                                break;
                        }
                    }
                }
            }

            Servicos.Log.GravarInfo(codigoAcerto.ToString() + " Fim ContemAbastecimentoPendenteAutorizacao " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            return contemResumoPendente;
        }

        public bool ContemAbastecimentoVeiculoPendenteAutorizacao(int codigoAcerto, int codigoVeiculo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento tipoAbastecimento, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Servicos.Log.GravarInfo(codigoAcerto.ToString() + " Inicio ContemAbastecimentoVeiculoPendenteAutorizacao " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");

            Repositorio.Embarcador.Acerto.AcertoResumoAbastecimento repAcertoResumoAbastecimento = new Repositorio.Embarcador.Acerto.AcertoResumoAbastecimento(unidadeDeTrabalho);
            Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(unidadeDeTrabalho);
            Repositorio.Embarcador.Acerto.AcertoAbastecimento repAcertoAbastecimento = new Repositorio.Embarcador.Acerto.AcertoAbastecimento(unidadeDeTrabalho);

            Dominio.Entidades.Embarcador.Acerto.AcertoResumoAbastecimento resumo = repAcertoResumoAbastecimento.BuscarPorCodigoAcertoVeiculoTipo(codigoAcerto, codigoVeiculo, tipoAbastecimento);

            if (tipoAbastecimento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Arla && resumo.Media > 0m)
            {
                Servicos.Log.GravarInfo(codigoAcerto.ToString() + " Fim ContemAbastecimentoVeiculoPendenteAutorizacao " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                return resumo.MediaIdeal <= 0m;
            }
            else if (tipoAbastecimento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Arla)
            {
                Servicos.Log.GravarInfo(codigoAcerto.ToString() + " Fim ContemAbastecimentoVeiculoPendenteAutorizacao " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                return false;
            }


            if (resumo.Media <= 0m || resumo.MediaIdeal <= 0m)
            {
                Servicos.Log.GravarInfo(codigoAcerto.ToString() + " Fim ContemAbastecimentoVeiculoPendenteAutorizacao " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                return true;
            }

            bool contemResumoPendente = (resumo.Media < resumo.MediaIdeal) && !resumo.ResumoAprovado;

            if (contemResumoPendente)
            {
                Servicos.Log.GravarInfo(codigoAcerto.ToString() + " Fim ContemAbastecimentoVeiculoPendenteAutorizacao " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                return contemResumoPendente;
            }
            else
            {
                List<Dominio.Entidades.Embarcador.Acerto.AcertoAbastecimento> listaAbastecimento = repAcertoAbastecimento.BuscarPorCodigoAcertoVeiculoTipoParaVerificarPendencias(codigoAcerto, codigoVeiculo, tipoAbastecimento);

                int countAbastecimentos = listaAbastecimento.Count;

                for (int i = 0; i < countAbastecimentos; i++)
                {
                    Dominio.Entidades.Embarcador.Acerto.AcertoAbastecimento abastecimento = listaAbastecimento[i];

                    if (abastecimento.Abastecimento.Situacao != "F")
                    {
                        Servicos.Log.GravarInfo(codigoAcerto.ToString() + " Fim ContemAbastecimentoVeiculoPendenteAutorizacao " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                        return contemResumoPendente;
                    }

                    contemResumoPendente = repAbastecimento.AbastecimentoDuplicado(abastecimento.Abastecimento);

                    if (contemResumoPendente)
                    {
                        contemResumoPendente = !repAcertoResumoAbastecimento.ResumoAutorizado(codigoAcerto, abastecimento.Abastecimento.Veiculo.Codigo, tipoAbastecimento);

                        if (contemResumoPendente)
                            break;
                    }
                }
            }

            Servicos.Log.GravarInfo(codigoAcerto.ToString() + " Fim ContemAbastecimentoVeiculoPendenteAutorizacao " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            return contemResumoPendente;
        }

        public bool ContemPedagioPendenteAutorizacao(int codigoAcerto, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Servicos.Log.GravarInfo(codigoAcerto.ToString() + " Inicio ContemPedagioPendenteAutorizacao " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            Repositorio.Embarcador.Acerto.AcertoPedagio repAcertoPedagio = new Repositorio.Embarcador.Acerto.AcertoPedagio(unidadeDeTrabalho);

            bool contemPedagioPendente = repAcertoPedagio.ContemPedagioDuplicado(codigoAcerto);

            if (contemPedagioPendente)
            {
                Servicos.Log.GravarInfo(codigoAcerto.ToString() + " Fim ContemPedagioPendenteAutorizacao " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                return contemPedagioPendente;
            }
            else
            {
                Servicos.Log.GravarInfo(codigoAcerto.ToString() + " Fim ContemPedagioPendenteAutorizacao " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                return repAcertoPedagio.ExistePorAcertoESituacaoDiff(codigoAcerto, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedagio.Fechado);
            }
        }

        public List<Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReceitaAcertoViagem> RetornaObjetoReceitaViagem(int codigoAcerto, Repositorio.UnitOfWork unitOfWork, bool NaoLancarDescontosDasOcorrenciasNoAcertoDeViagem, bool acertoDeViagemImpressaoDetalhada, bool gerarTituloPagarFolhaFuncionario, bool gerarReciboAcertoViagemDetalhado, bool visualizarReciboPorMotoristaNoAcertoDeViagem, bool separarValoresAdiantamentoMotoristaPorTipo)
        {
            Servicos.Log.GravarInfo(codigoAcerto.ToString() + " Inicio RetornaObjetoReceitaViagem " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");

            Repositorio.Embarcador.Acerto.TabelaComissaoMotorista repTabelaComissaoMotorista = new Repositorio.Embarcador.Acerto.TabelaComissaoMotorista(unitOfWork);
            Repositorio.Embarcador.Acerto.AcertoViagemTabelaComissao repAcertoViagemTabelaComissao = new Repositorio.Embarcador.Acerto.AcertoViagemTabelaComissao(unitOfWork);
            Repositorio.Embarcador.Acerto.AcertoViagemInfracao repAcertoViagemInfracao = new Repositorio.Embarcador.Acerto.AcertoViagemInfracao(unitOfWork);
            Repositorio.Embarcador.Acerto.AcertoFolhaLancamento repAcertoFolhaLancamento = new Repositorio.Embarcador.Acerto.AcertoFolhaLancamento(unitOfWork);
            Repositorio.Embarcador.Acerto.AcertoOutraDespesa repDespesa = new Repositorio.Embarcador.Acerto.AcertoOutraDespesa(unitOfWork);
            Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unitOfWork);
            Repositorio.Embarcador.Acerto.AcertoCarga repCarga = new Repositorio.Embarcador.Acerto.AcertoCarga(unitOfWork);
            Repositorio.Embarcador.Acerto.AcertoResumoAbastecimento repResumo = new Repositorio.Embarcador.Acerto.AcertoResumoAbastecimento(unitOfWork);
            Repositorio.Embarcador.Acerto.AcertoAbastecimento repAcertoAbastecimento = new Repositorio.Embarcador.Acerto.AcertoAbastecimento(unitOfWork);
            Repositorio.Embarcador.Acerto.AcertoResumoAbastecimento repAcertoResumoAbastecimento = new Repositorio.Embarcador.Acerto.AcertoResumoAbastecimento(unitOfWork);
            Repositorio.Embarcador.Acerto.AcertoCargaPedagio repAcertoCargaPedagio = new Repositorio.Embarcador.Acerto.AcertoCargaPedagio(unitOfWork);
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS repPagamentoMotoristaTMS = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS(unitOfWork);
            Repositorio.Embarcador.Acerto.AcertoVeiculoResultado repAcertoVeiculoResultado = new Repositorio.Embarcador.Acerto.AcertoVeiculoResultado(unitOfWork);
            Repositorio.Embarcador.Acerto.AcertoAdiantamento repAcertoAdiantamento = new Repositorio.Embarcador.Acerto.AcertoAdiantamento(unitOfWork);
            Repositorio.Embarcador.Acerto.AcertoBonificacao repAcertoBonificacao = new Repositorio.Embarcador.Acerto.AcertoBonificacao(unitOfWork);
            Repositorio.Embarcador.Acerto.AcertoDesconto repAcertoDesconto = new Repositorio.Embarcador.Acerto.AcertoDesconto(unitOfWork);
            Repositorio.Embarcador.Frota.Infracao repInfracao = new Repositorio.Embarcador.Frota.Infracao(unitOfWork);
            Repositorio.Embarcador.RH.FolhaLancamento repFolhaLancamento = new Repositorio.Embarcador.RH.FolhaLancamento(unitOfWork);

            Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem = repAcertoViagem.BuscarPorCodigo(codigoAcerto);
            Dominio.Entidades.Embarcador.Acerto.AcertoViagemTabelaComissao acertoViagemTabelaComissao = repAcertoViagemTabelaComissao.BuscarPorAcerto(codigoAcerto);
            List<Dominio.Entidades.Embarcador.Acerto.AcertoViagemInfracao> infracoesMotorista = repAcertoViagemInfracao.BuscarPorAcerto(codigoAcerto);
            Dominio.Entidades.Embarcador.Acerto.TabelaComissaoMotorista tabelaComissao = null;
            Dominio.Entidades.Embarcador.Acerto.TabelaComissaoMotoristaMedia media = null;
            Dominio.Entidades.Embarcador.Acerto.TabelaComissaoFaturamentoDia faturamentoDia = null;
            Dominio.Entidades.Embarcador.Acerto.TabelaComissaoMotoristaRepresentacao representacao = null;
            List<Dominio.Entidades.Embarcador.Acerto.TabelaComissaoMotoristaRotaFrete> rotasFrete = null;

            decimal percentualComissaoTabela = 10;
            decimal valorPremioComissao = 0;
            decimal percentualPremioComissao = 0;
            decimal valorFreteLiquido = repCarga.ValorFreteLiquido(acertoViagem.Codigo, 0);
            valorFreteLiquido += repCarga.ValorFreteLiquidoFracionadoAcerto(acertoViagem.Codigo);
            decimal despesaCombustivel = acertoViagem.Abastecimentos != null ? (from obj in acertoViagem.Abastecimentos where obj.Abastecimento.TipoAbastecimento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Combustivel select obj.Abastecimento.ValorTotal).Sum() : 0;
            decimal mediaKM = repAcertoResumoAbastecimento.BuscarMediaDoAcerto(acertoViagem.Codigo);

            if (acertoViagem.Veiculos != null)
            {
                Dominio.Entidades.Veiculo veiculoTracao = acertoViagem.Veiculos.Where(o => o.Veiculo.TipoVeiculo == "0").Select(o => o.Veiculo).FirstOrDefault();

                if (veiculoTracao != null)
                {
                    List<int> codigosTipoOperacao = repCarga.BuscarCodigosTipoOperacao(acertoViagem.Codigo);

                    tabelaComissao = repTabelaComissaoMotorista.BuscarPorTabelaAcertoViagem(acertoViagem.DataFinal.HasValue ? acertoViagem.DataFinal.Value : acertoViagem.DataInicial, acertoViagem.SegmentoVeiculo?.Codigo ?? 0, veiculoTracao.Modelo?.Codigo ?? 0, codigosTipoOperacao);
                    if (tabelaComissao == null)
                        tabelaComissao = repTabelaComissaoMotorista.BuscarPorTabelaAcertoViagem(acertoViagem.DataFinal.HasValue ? acertoViagem.DataFinal.Value : acertoViagem.DataInicial, acertoViagem.SegmentoVeiculo?.Codigo ?? 0, 0, null);
                    if (tabelaComissao == null)
                        tabelaComissao = repTabelaComissaoMotorista.BuscarPorTabelaAcertoViagem(acertoViagem.DataFinal.HasValue ? acertoViagem.DataFinal.Value : acertoViagem.DataInicial, 0, veiculoTracao.Modelo?.Codigo ?? 0, null);
                    if (tabelaComissao == null)
                        tabelaComissao = repTabelaComissaoMotorista.BuscarPorTabelaAcertoViagem(acertoViagem.DataFinal.HasValue ? acertoViagem.DataFinal.Value : acertoViagem.DataInicial, 0, 0, codigosTipoOperacao);
                    if (tabelaComissao == null)
                        tabelaComissao = repTabelaComissaoMotorista.BuscarPorTabelaAcertoViagem(acertoViagem.DataFinal.HasValue ? acertoViagem.DataFinal.Value : acertoViagem.DataInicial, 0, 0, null);

                    if (tabelaComissao != null)
                    {
                        percentualComissaoTabela = tabelaComissao.PercentualComissaoPadrao;
                        if (tabelaComissao.AtivarBonificacaoMediaCombustivel && tabelaComissao.Medias != null && tabelaComissao.Medias.Count > 0 && mediaKM > 0)
                        {
                            media = tabelaComissao.Medias.Where(o => (o.MediaInicial <= mediaKM) && (o.MediaFinal >= mediaKM)).FirstOrDefault();
                        }
                        else if (tabelaComissao.AtivarBonificacaoRepresentacaoCombustivel && tabelaComissao.Representacaos != null && tabelaComissao.Representacaos.Count > 0)
                        {
                            decimal percentualRepresentacao = 0;
                            if (valorFreteLiquido > 0 && despesaCombustivel > 0)
                                percentualRepresentacao = Math.Round(((despesaCombustivel * (decimal)100) / valorFreteLiquido), 2);
                            representacao = tabelaComissao.Representacaos.Where(o => o.PercentualRepresentacao == percentualRepresentacao).FirstOrDefault();
                        }
                        else if (tabelaComissao.AtivarBonificacaoFaturamentoDia && tabelaComissao.FaturamentoDia != null && tabelaComissao.FaturamentoDia.Count > 0)
                        {
                            int diasAcerto = (int)(acertoViagem.DataFinal.Value - acertoViagem.DataInicial).TotalDays;
                            decimal freteBrutoCalculoComissao = acertoViagem.Cargas != null ? (from obj in acertoViagem.Cargas select obj.ValorBrutoCarga).Sum() : 0;
                            decimal impostosCalculoComissao = repCarga.ValorICMSAcerto(acertoViagem.Codigo, true, 0);
                            impostosCalculoComissao += repCarga.ValorICMSFracionadoAcerto(acertoViagem.Codigo, 0);
                            decimal valorComponentePedagioCalculoComissao = repCarga.ValorComponenteFrete(acertoViagem.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PEDAGIO);

                            decimal freteBrutoCalculado = (freteBrutoCalculoComissao + impostosCalculoComissao + valorComponentePedagioCalculoComissao) / diasAcerto;
                            faturamentoDia = tabelaComissao.FaturamentoDia.Where(o => (o.FaturamentoInicial <= freteBrutoCalculado) && (o.FaturamentoFinal >= freteBrutoCalculado)).FirstOrDefault();
                            if (faturamentoDia != null)
                            {
                                percentualPremioComissao = faturamentoDia.PercentualAcrescimoComissao;
                                decimal valorComissionado = freteBrutoCalculado - faturamentoDia.FaturamentoInicial;
                                valorPremioComissao = ((valorComissionado * diasAcerto) * faturamentoDia.PercentualAcrescimoComissao) / 100;
                            }
                        }
                        else if (tabelaComissao.AtivarBonificacaoRotaFrete && tabelaComissao.RotasFretes != null && tabelaComissao.RotasFretes.Count > 0 && acertoViagem.Cargas != null && acertoViagem.Cargas.Count > 0)
                        {
                            rotasFrete = tabelaComissao.RotasFretes.Where(o => acertoViagem.Cargas.Any(c => c.Carga.Rota == o.RotaFrete)).Distinct().ToList();
                        }
                        if (media != null)
                            percentualComissaoTabela += media.PercentualAcrescimoComissao;
                        else if (representacao != null)
                            percentualComissaoTabela += representacao.PercentualAcrescimoComissao;

                        if (acertoViagemTabelaComissao == null && acertoViagem.Situacao == SituacaoAcertoViagem.EmAntamento)
                        {
                            acertoViagemTabelaComissao = new Dominio.Entidades.Embarcador.Acerto.AcertoViagemTabelaComissao()
                            {
                                AcertoViagem = acertoViagem,
                                TabelaComissaoMotoristaMedia = media,
                                TabelaComissaoMotoristaRepresentacao = representacao,
                                TabelaComissaoFaturamentoDia = faturamentoDia,
                                RotasFrete = rotasFrete
                            };
                            if (rotasFrete != null && rotasFrete.Count > 0)
                            {
                                foreach (var rotaFrete in rotasFrete)
                                {
                                    if (rotaFrete.JustificativaBonificacao != null && rotaFrete.ValorBonificacao > 0)
                                    {
                                        Dominio.Entidades.Embarcador.Acerto.AcertoBonificacao bonificacaoTabela = new Dominio.Entidades.Embarcador.Acerto.AcertoBonificacao()
                                        {
                                            AcertoViagem = acertoViagem,
                                            Data = acertoViagem.DataFinal.HasValue ? acertoViagem.DataFinal.Value : acertoViagem.DataInicial,
                                            Justificativa = rotaFrete.JustificativaBonificacao,
                                            Motivo = ("RECEBIDO DA TABELA DE COMISSÃO DO MOTORISTA PELA ROTA " + rotaFrete.RotaFrete?.Descricao ?? ""),
                                            TipoBonificacao = null,
                                            ValorBonificacao = rotaFrete.ValorBonificacao,
                                            Veiculo = veiculoTracao
                                        };
                                        repAcertoBonificacao.Inserir(bonificacaoTabela);
                                    }
                                }
                            }
                            if (acertoViagemTabelaComissao.TabelaComissaoMotoristaMedia != null && acertoViagemTabelaComissao.TabelaComissaoMotoristaMedia.ValorBonificacao > 0 && acertoViagemTabelaComissao.TabelaComissaoMotoristaMedia.JustificativaBonificacao != null)
                            {
                                Dominio.Entidades.Embarcador.Acerto.AcertoBonificacao bonificacaoTabela = new Dominio.Entidades.Embarcador.Acerto.AcertoBonificacao()
                                {
                                    AcertoViagem = acertoViagem,
                                    Data = acertoViagem.DataFinal.HasValue ? acertoViagem.DataFinal.Value : acertoViagem.DataInicial,
                                    Justificativa = acertoViagemTabelaComissao.TabelaComissaoMotoristaMedia.JustificativaBonificacao,
                                    Motivo = "RECEBIDO DA TABELA DE COMISSÃO DO MOTORISTA",
                                    TipoBonificacao = null,
                                    ValorBonificacao = acertoViagemTabelaComissao.TabelaComissaoMotoristaMedia.ValorBonificacao,
                                    Veiculo = veiculoTracao
                                };
                                repAcertoBonificacao.Inserir(bonificacaoTabela);
                            }
                            else if (acertoViagemTabelaComissao.TabelaComissaoMotoristaRepresentacao != null && acertoViagemTabelaComissao.TabelaComissaoMotoristaRepresentacao.ValorBonificacao > 0 && acertoViagemTabelaComissao.TabelaComissaoMotoristaRepresentacao.JustificativaBonificacao != null)
                            {
                                Dominio.Entidades.Embarcador.Acerto.AcertoBonificacao bonificacaoTabela = new Dominio.Entidades.Embarcador.Acerto.AcertoBonificacao()
                                {
                                    AcertoViagem = acertoViagem,
                                    Data = acertoViagem.DataFinal.HasValue ? acertoViagem.DataFinal.Value : acertoViagem.DataInicial,
                                    Justificativa = acertoViagemTabelaComissao.TabelaComissaoMotoristaRepresentacao.JustificativaBonificacao,
                                    Motivo = "RECEBIDO DA TABELA DE COMISSÃO DO MOTORISTA",
                                    TipoBonificacao = null,
                                    ValorBonificacao = acertoViagemTabelaComissao.TabelaComissaoMotoristaRepresentacao.ValorBonificacao,
                                    Veiculo = veiculoTracao
                                };
                                repAcertoBonificacao.Inserir(bonificacaoTabela);
                            }
                            repAcertoViagemTabelaComissao.Inserir(acertoViagemTabelaComissao);
                        }
                        else if (acertoViagem.Situacao == SituacaoAcertoViagem.EmAntamento)
                        {
                            acertoViagemTabelaComissao.TabelaComissaoMotoristaMedia = media;
                            acertoViagemTabelaComissao.TabelaComissaoMotoristaRepresentacao = representacao;
                            acertoViagemTabelaComissao.TabelaComissaoFaturamentoDia = faturamentoDia;
                            acertoViagemTabelaComissao.RotasFrete = rotasFrete;

                            if (media == null && representacao == null && faturamentoDia == null && rotasFrete == null)
                                repAcertoViagemTabelaComissao.Deletar(acertoViagemTabelaComissao);
                            else
                                repAcertoViagemTabelaComissao.Atualizar(acertoViagemTabelaComissao);
                        }
                        acertoViagem = repAcertoViagem.BuscarPorCodigo(codigoAcerto);
                    }
                }

                //if (NaoLancarDescontosDasOcorrenciasNoAcertoDeViagem && acertoViagem.Situacao == SituacaoAcertoViagem.EmAntamento)
                //{
                //    List<Dominio.Entidades.Embarcador.Frota.Infracao> listaInfracoesMotorista = repInfracao.BuscarInfracoesMotorista(true, null, null, acertoViagem.Motorista.Codigo, acertoViagem.Codigo);
                //    //List<Dominio.Entidades.Embarcador.Frota.Infracao> listaInfracoesMotorista = repInfracao.BuscarInfracoesMotorista(acertoViagem.Motorista?.Codigo ?? 0, acertoViagem.DataInicial, acertoViagem.DataFinal);
                //    if (listaInfracoesMotorista.Count > 0 && veiculoTracao != null)
                //    {
                //        foreach (var infracao in listaInfracoesMotorista)
                //        {
                //            if (infracoesMotorista == null || infracoesMotorista.Count == 0 || !infracoesMotorista.Select(o => o.Infracao).Contains(infracao))
                //            {
                //                if (infracao.ReduzirPercentualComissaoMotorista && infracao.PercentualReducaoComissaoMotorista > 0)
                //                    percentualComissaoTabela -= infracao.PercentualReducaoComissaoMotorista;
                //                if (infracao.LancarDescontoMotorista && infracao.DescontoComissaoMotorista > 0 && infracao.JustificativaDesconto != null)
                //                {
                //                    Dominio.Entidades.Embarcador.Acerto.AcertoDesconto descontoTabela = new Dominio.Entidades.Embarcador.Acerto.AcertoDesconto()
                //                    {
                //                        AcertoViagem = acertoViagem,
                //                        Data = acertoViagem.DataFinal.HasValue ? acertoViagem.DataFinal.Value : acertoViagem.DataInicial,
                //                        Justificativa = infracao.JustificativaDesconto,
                //                        Motivo = "DESCONTO LANÇADO DEVIDO OCORRÊNCIA AO MOTORISTA Nº " + infracao.Numero.ToString(),
                //                        ValorDesconto = infracao.DescontoComissaoMotorista,
                //                        Veiculo = veiculoTracao
                //                    };
                //                    repAcertoDesconto.Inserir(descontoTabela);
                //                }

                //                Dominio.Entidades.Embarcador.Acerto.AcertoViagemInfracao acertoViagemInfracao = new Dominio.Entidades.Embarcador.Acerto.AcertoViagemInfracao()
                //                {
                //                    AcertoViagem = acertoViagem,
                //                    Infracao = infracao,
                //                    InfracaoAssinada = false
                //                };
                //                repAcertoViagemInfracao.Inserir(acertoViagemInfracao);
                //            }
                //        }
                //    }
                //}
                if (percentualComissaoTabela < 0)
                    percentualComissaoTabela = 0;
            }

            if (!gerarTituloPagarFolhaFuncionario && acertoViagem.Situacao == SituacaoAcertoViagem.EmAntamento)
            {
                List<Dominio.Entidades.Embarcador.Acerto.AcertoFolhaLancamento> folhasLanamento = repAcertoFolhaLancamento.BuscarPorAcerto(codigoAcerto);
                List<Dominio.Entidades.Embarcador.RH.FolhaLancamento> listaFolhaLancamento = repFolhaLancamento.BuscarFolhaLancamentoMotorista(acertoViagem.Motorista?.Codigo ?? 0, acertoViagem.DataInicial, acertoViagem.DataFinal);
                if (acertoViagem.Veiculos != null && listaFolhaLancamento.Count > 0 && acertoViagem.Motorista != null)
                {
                    Dominio.Entidades.Veiculo veiculoTracao = acertoViagem.Veiculos.Where(o => o.Veiculo.TipoVeiculo == "0").Select(o => o.Veiculo).FirstOrDefault();
                    if (veiculoTracao != null)
                    {
                        bool inserirFolhaLancamento = false;
                        foreach (var folhaLancamento in listaFolhaLancamento)
                        {
                            if (!folhasLanamento.Select(o => o.FolhaLancamento).Contains(folhaLancamento))
                            {
                                if (folhaLancamento.FolhaInformacao != null && folhaLancamento.FolhaInformacao.Justificativa != null && folhaLancamento.FolhaInformacao.Justificativa.TipoJustificativa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Desconto)
                                {
                                    Dominio.Entidades.Embarcador.Acerto.AcertoDesconto descontoTabela = new Dominio.Entidades.Embarcador.Acerto.AcertoDesconto()
                                    {
                                        AcertoViagem = acertoViagem,
                                        Data = acertoViagem.DataFinal.HasValue ? acertoViagem.DataFinal.Value : acertoViagem.DataInicial,
                                        Justificativa = folhaLancamento.FolhaInformacao.Justificativa,
                                        Motivo = "DESCONTO REF. IMP. DADOS DO RH: " + folhaLancamento.Descricao,
                                        ValorDesconto = folhaLancamento.Valor,
                                        Veiculo = veiculoTracao
                                    };
                                    repAcertoDesconto.Inserir(descontoTabela);
                                    inserirFolhaLancamento = true;
                                }
                                else if (folhaLancamento.FolhaInformacao != null && folhaLancamento.FolhaInformacao.Justificativa != null && folhaLancamento.FolhaInformacao.Justificativa.TipoJustificativa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Acrescimo)
                                {
                                    Dominio.Entidades.Embarcador.Acerto.AcertoBonificacao bonificacaoTabela = new Dominio.Entidades.Embarcador.Acerto.AcertoBonificacao()
                                    {
                                        AcertoViagem = acertoViagem,
                                        Data = acertoViagem.DataFinal.HasValue ? acertoViagem.DataFinal.Value : acertoViagem.DataInicial,
                                        Justificativa = folhaLancamento.FolhaInformacao.Justificativa,
                                        Motivo = "RECEBIDO REF. IMP. DADOS DO RH: " + folhaLancamento.Descricao,
                                        TipoBonificacao = null,
                                        ValorBonificacao = folhaLancamento.Valor,
                                        Veiculo = veiculoTracao
                                    };
                                    repAcertoBonificacao.Inserir(bonificacaoTabela);
                                    inserirFolhaLancamento = true;
                                }
                                if (inserirFolhaLancamento)
                                {
                                    Dominio.Entidades.Embarcador.Acerto.AcertoFolhaLancamento acertoFolhaLancamento = new Dominio.Entidades.Embarcador.Acerto.AcertoFolhaLancamento()
                                    {
                                        AcertoViagem = acertoViagem,
                                        FolhaLancamento = folhaLancamento
                                    };
                                    repAcertoFolhaLancamento.Inserir(acertoFolhaLancamento);
                                }
                            }
                        }
                    }
                }

            }
            Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReceitaAcertoViagem receita = new Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReceitaAcertoViagem();
            receita.CodigoAcerto = acertoViagem.Codigo;
            receita.Motorista = acertoViagem.Motorista.Nome;
            receita.CPFMotorista = acertoViagem.Motorista.CPF_Formatado;
            receita.Periodo = acertoViagem.DescricaoPeriodo;
            receita.NumeroViagens = acertoViagem.Cargas != null ? (from obj in acertoViagem.Cargas select obj).Count() : 0;
            receita.NumeroViagensCompartilhada = repCarga.QuantidadeViagensCompartilhadas(acertoViagem.Codigo);
            receita.ValorViagensCompartilhada = acertoViagem.Cargas != null ? (from obj in acertoViagem.Cargas where obj.PercentualAcerto < 100 select obj.ValorBrutoCarga).Sum() : 0;
            receita.PlacasVeiculos = acertoViagem.Veiculos != null ? string.Join(", ", (from obj in acertoViagem.Veiculos select obj.Veiculo.Placa)) : string.Empty;
            receita.PlacasReboques = "";
            if (acertoViagem.DataFinal.HasValue)
                receita.DiasViagem = (int)(acertoViagem.DataFinal.Value - acertoViagem.DataInicial).TotalDays;
            else
                receita.DiasViagem = 1;

            receita.DespesaCombustivel = acertoViagem.Abastecimentos != null ? (from obj in acertoViagem.Abastecimentos where obj.Abastecimento.TipoAbastecimento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Combustivel select obj.Abastecimento.ValorTotal).Sum() : 0;
            receita.DespesaArla = acertoViagem.Abastecimentos != null ? (from obj in acertoViagem.Abastecimentos where obj.Abastecimento.TipoAbastecimento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Arla select obj.Abastecimento.ValorTotal).Sum() : 0;
            receita.PedagioPago = acertoViagem.Pedagios != null ? (from obj in acertoViagem.Pedagios where obj.Pedagio.TipoPedagio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedagio.Debito select obj.Pedagio.Valor).Sum() : 0;
            receita.DespesaMotorista = acertoViagem.OutrasDespesas != null ? (from obj in acertoViagem.OutrasDespesas where obj.Quantidade > 0 select obj.Valor * obj.Quantidade).Sum() : 0;
            receita.DespesaMotorista += acertoViagem.OutrasDespesas != null ? (from obj in acertoViagem.OutrasDespesas where obj.Quantidade == 0 select obj.Valor).Sum() : 0;
            receita.TotalDespesa = receita.DespesaCombustivel + receita.DespesaArla + receita.PedagioPago + receita.DespesaMotorista;

            decimal freteBruto = acertoViagem.Cargas != null ? (from obj in acertoViagem.Cargas select obj.ValorBrutoCarga).Sum() : 0;

            decimal valorCTeComplementar = repCarga.ValorFreteLiquidoComplementar(acertoViagem.Codigo, 0);

            decimal valorComponentePedagio = repCarga.ValorComponenteFrete(acertoViagem.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PEDAGIO);
            decimal valorComponenteSubtrair = repCarga.ValorComponenteFretePorTipoAcerto(acertoViagem.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoAcertoViagem.Subtrair, false);
            decimal valorComponenteSomar = repCarga.ValorComponenteFretePorTipoAcerto(acertoViagem.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoAcertoViagem.Somar, false);

            decimal impostos = repCarga.ValorICMSAcerto(acertoViagem.Codigo, true, 0);
            impostos += repCarga.ValorICMSFracionadoAcerto(acertoViagem.Codigo, 0);

            receita.ReceitaFrete = valorFreteLiquido;// - impostos;// + valorCTeComplementar;// freteBruto - impostos - valorComponentePedagio;

            decimal receitaPedagio = acertoViagem.Cargas != null ? (from obj in acertoViagem.Cargas select obj.PedagioAcerto).Sum() : 0;
            receitaPedagio += acertoViagem.Cargas != null ? (from obj in acertoViagem.Cargas select obj.PedagioAcertoCredito).Sum() : 0;
            receitaPedagio += repAcertoCargaPedagio.TotalValorPedagio(codigoAcerto);
            receitaPedagio += acertoViagem.Pedagios != null ? (from obj in acertoViagem.Pedagios where obj.Pedagio.TipoPedagio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedagio.Credito select obj.Pedagio.Valor).Sum() : 0;
            receita.PedagioRecebido = valorComponentePedagio + receitaPedagio;
            receita.OutrosRecebimentos = valorComponenteSomar - valorComponenteSubtrair;
            receita.BonificacaoCliente = acertoViagem.Cargas != null ? (from obj in acertoViagem.Cargas select obj.ValorBonificacaoCliente).Sum() : 0;
            receita.Ocorrencias = acertoViagem.Ocorrencias != null ? (from obj in acertoViagem.Ocorrencias select obj.CargaOcorrencia.ValorOcorrencia).Sum() : 0;
            receita.TotalReceita = receita.ReceitaFrete + receita.PedagioRecebido + receita.OutrosRecebimentos + receita.BonificacaoCliente + receita.Ocorrencias;

            receita.TotalSaldo = receita.TotalReceita - receita.TotalDespesa;

            receita.AbastecimentoMotorista = acertoViagem.Abastecimentos != null ? (from obj in acertoViagem.Abastecimentos where obj.Abastecimento.Posto != null && obj.Abastecimento.Posto.Modalidades.Any(o => o.TipoModalidade == TipoModalidade.Fornecedor && o.ModalidadesFornecedores.Any(p => !p.PagoPorFatura)) select obj.Abastecimento.ValorTotal).Sum() : 0;
            receita.PedagioMotorista = acertoViagem.Pedagios != null ? (from obj in acertoViagem.Pedagios where obj.Pedagio.TipoPedagio == TipoPedagio.Debito && !obj.Pedagio.ImportadoDeSemParar select obj.Pedagio.Valor).Sum() : 0;
            receita.OutraDespesaMotorista = acertoViagem.OutrasDespesas != null ? (from obj in acertoViagem.OutrasDespesas where obj.TipoPagamento == TipoPagamentoAcertoDespesa.Motorista || (obj.TipoPagamento != TipoPagamentoAcertoDespesa.Empresa && obj.Pessoa.Modalidades.Any(o => o.TipoModalidade == TipoModalidade.Fornecedor && o.ModalidadesFornecedores.Any(p => !p.PagoPorFatura))) select obj.Valor * (obj.Quantidade > 0 ? obj.Quantidade : 1)).Sum() : 0;
            if (gerarReciboAcertoViagemDetalhado)
                receita.OutraDespesaMotorista = acertoViagem.OutrasDespesas != null ? (from obj in acertoViagem.OutrasDespesas where obj.TipoPagamento != TipoPagamentoAcertoDespesa.Empresa select obj.Valor * (obj.Quantidade > 0 ? obj.Quantidade : 1)).Sum() : 0;

            receita.AdiantamentoMotorista = 0;
            receita.RetornoAdiantamento = 0;
            if (separarValoresAdiantamentoMotoristaPorTipo)
            {
                receita.AdiantamentoMotorista = repAcertoAdiantamento.BuscarValorTotalPorAcertoETipo(acertoViagem.Codigo, TipoMovimentoEntidade.Entrada);
                receita.RetornoAdiantamento = repAcertoAdiantamento.BuscarValorTotalPorAcertoETipo(acertoViagem.Codigo, TipoMovimentoEntidade.Saida);
            }
            else
                receita.AdiantamentoMotorista = repAcertoAdiantamento.BuscarValorTotalPorAcerto(acertoViagem.Codigo);

            if (receita.AdiantamentoMotorista == 0 && acertoViagem.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem.Fechado)
            {
                List<Dominio.Entidades.Embarcador.Acerto.AcertoVeiculoResultado> resultado = repAcertoVeiculoResultado.BuscarPorAcerto(acertoViagem.Codigo);
                if (resultado != null && resultado.Count > 0)
                    receita.AdiantamentoMotorista = resultado.Select(o => o.AdiantamentoMotorista).FirstOrDefault();
                else
                    receita.AdiantamentoMotorista = 0;
            }
            receita.DiariaMotorista = acertoViagem.Diarias != null ? (from obj in acertoViagem.Diarias select obj.Valor).Sum() : 0;
            receita.BonificacoesMotorista = acertoViagem.Bonificacoes != null ? (from obj in acertoViagem.Bonificacoes select obj.ValorBonificacao).Sum() : 0;
            receita.DescontosMotorista = acertoViagem.Descontos != null ? (from obj in acertoViagem.Descontos select obj.ValorDesconto).Sum() : 0;
            receita.DevolucoesMotorista = acertoViagem.DevolucoesMoedaEstrangeira != null ? (from obj in acertoViagem.DevolucoesMoedaEstrangeira select obj.ValorOriginal).Sum() : 0;
            //receita.VariacaoCambial = acertoViagem.VariacoesCambial != null ? (from obj in acertoViagem.VariacoesCambial select obj.ValorOriginal).Sum() : 0;
            receita.VariacaoCambial = acertoViagem.VariacaoCambial;
            receita.VariacaoCambialReceita = acertoViagem.VariacaoCambialReceita;

            receita.TotalDespesaMotorista = receita.RetornoAdiantamento + receita.DiariaMotorista + receita.AbastecimentoMotorista + receita.PedagioMotorista + receita.OutraDespesaMotorista + receita.BonificacoesMotorista + receita.DevolucoesMotorista + receita.VariacaoCambial;
            receita.TotalReceitaMotorista = receita.AdiantamentoMotorista + receita.DescontosMotorista + receita.VariacaoCambialReceita;
            receita.SaldoMotorista = receita.TotalReceitaMotorista - receita.TotalDespesaMotorista;

            receita.FaturamentoBruto = freteBruto + valorComponentePedagio;
            receita.FaturamentoLiquido = receita.ReceitaFrete;// + impostos;
            receita.TotalImposto = impostos;
            if (acertoViagem.Motorista.NaoGeraComissaoAcerto)
            {
                receita.ComissaoMotorista = 0;
                receita.PercentualComissao = 0;
                receita.PercentualPremioComissao = 0;
                receita.PremioComissaoMotorista = 0;
            }
            else
            {
                receita.ComissaoMotorista = (receita.FaturamentoLiquido * percentualComissaoTabela) / 100;
                receita.PercentualComissao = percentualComissaoTabela;
                if (acertoDeViagemImpressaoDetalhada && receita.Ocorrencias > 0 && percentualComissaoTabela > 0)
                    receita.ComissaoMotorista += ((receita.Ocorrencias * percentualComissaoTabela) / 100);

                receita.PercentualPremioComissao = percentualPremioComissao;
                receita.PremioComissaoMotorista = valorPremioComissao;
            }

            if (visualizarReciboPorMotoristaNoAcertoDeViagem)
            {
                receita.TotalReceitaMotorista -= receita.ComissaoMotorista;
                receita.SaldoMotorista -= receita.ComissaoMotorista;

                receita.SaldoMotorista = (receita.SaldoMotorista * -1);
            }

            receita.ValorBonificacao = 0;
            receita.MotivoBonificacao = "";

            receita.ValorTotalBonificacao = acertoViagem.Bonificacoes != null ? (from obj in acertoViagem.Bonificacoes select obj.ValorBonificacao).Sum() : 0;
            receita.ValorTotalDesconto = acertoViagem.Descontos != null ? (from obj in acertoViagem.Descontos select obj.ValorDesconto).Sum() : 0;

            receita.AdiantamentoXDespesas = repDespesa.ReceitaDespesaOutraDespesa(acertoViagem.Codigo, false);
            receita.TotalPagarMotorista = receita.ValorTotalBonificacao + receita.ComissaoMotorista + receita.AdiantamentoXDespesas - receita.ValorTotalDesconto;

            if (receita.DiasViagem > 0)
                receita.ValorLiquidoMes = (receita.TotalSaldo + receita.TotalImposto - receita.ComissaoMotorista - receita.ValorTotalBonificacao) / (decimal)receita.DiasViagem * 30;
            else
                receita.ValorLiquidoMes = 0;

            receita.ValorKMDiesel = 0;
            receita.ValorKMLiquido = 0;
            receita.MediaKM = 0;
            receita.Parametro = 0;
            decimal valorDieselTracao = repAcertoAbastecimento.ValorTotalAbastecimentos(acertoViagem.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Combustivel);
            int totalKMTracao = repAcertoResumoAbastecimento.BuscarKMTotalAcerto(acertoViagem.Codigo);
            if (valorDieselTracao > 0 && totalKMTracao > 0)
                receita.ValorKMDiesel = valorDieselTracao / totalKMTracao;
            receita.MediaKM = repAcertoResumoAbastecimento.BuscarMediaDoAcerto(acertoViagem.Codigo);
            receita.Parametro = repAcertoResumoAbastecimento.BuscarParametroDoAcerto(acertoViagem.Codigo);

            receita.TotalDespesa = receita.TotalDespesa + receita.ComissaoMotorista + receita.ValorTotalBonificacao;
            receita.TotalSaldo = receita.TotalReceita - receita.TotalDespesa;

            if (totalKMTracao > 0)
                receita.ValorKMLiquido = receita.TotalSaldo / totalKMTracao;

            List<Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReceitaAcertoViagem> retornoReceitas = new List<Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReceitaAcertoViagem>();

            List<Dominio.Entidades.Embarcador.Acerto.AcertoBonificacao> bonificacoes = acertoViagem.Bonificacoes != null ? (from obj in acertoViagem.Bonificacoes select obj).ToList() : null;
            if (bonificacoes.Count() > 0)
            {
                receita.MotivoBonificacao = bonificacoes[0].Motivo;
                receita.ValorBonificacao = bonificacoes[0].ValorBonificacao;
            }

            retornoReceitas.Add(receita);

            for (int i = 1; i < bonificacoes.Count(); i++)
            {
                Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReceitaAcertoViagem receitaBonificacao = new Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReceitaAcertoViagem();
                receitaBonificacao.CodigoAcerto = acertoViagem.Codigo;
                receitaBonificacao.MotivoBonificacao = bonificacoes[i].Motivo;
                receitaBonificacao.ValorBonificacao = bonificacoes[i].ValorBonificacao;
                receitaBonificacao.TotalDespesa = receita.TotalDespesa;
                receitaBonificacao.TotalReceita = receita.TotalReceita;
                receitaBonificacao.TotalSaldo = receita.TotalSaldo;
                receitaBonificacao.ValorLiquidoMes = receita.ValorLiquidoMes;

                retornoReceitas.Add(receitaBonificacao);
            }

            Servicos.Log.GravarInfo(codigoAcerto.ToString() + " Fim RetornaObjetoReceitaViagem " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
            return retornoReceitas;
        }

        public void AtualizarVeiculosSegmento(Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Servicos.Log.GravarInfo(acertoViagem?.Codigo.ToString() + " Inicio AtualizarVeiculosSegmento " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");

            Repositorio.Embarcador.Acerto.AcertoVeiculoSegmento repAcertoVeiculoSegmento = new Repositorio.Embarcador.Acerto.AcertoVeiculoSegmento(unidadeDeTrabalho);
            Repositorio.Embarcador.Acerto.AcertoCarga repAcertoCarga = new Repositorio.Embarcador.Acerto.AcertoCarga(unidadeDeTrabalho);

            //repAcertoVeiculoSegmento.DeletarPorAcerto(acertoViagem.Codigo);
            List<Dominio.Entidades.Embarcador.Acerto.AcertoVeiculoSegmento> segmentos = repAcertoVeiculoSegmento.BuscarPorAcerto(acertoViagem.Codigo);
            foreach (var segmento in segmentos)
                repAcertoVeiculoSegmento.Deletar(segmento);

            List<int> codigosVeiculos = new List<int>();
            List<Dominio.Entidades.Embarcador.Acerto.AcertoCarga> listaCargas = repAcertoCarga.BuscarPorCodigoAcerto(acertoViagem.Codigo);

            for (int i = 0; i < listaCargas.Count; i++)
            {
                Dominio.Entidades.Embarcador.Acerto.AcertoCarga acertoCarga = listaCargas[i];

                if (acertoCarga.Carga.Veiculo != null && !codigosVeiculos.Contains(acertoCarga.Carga.Veiculo.Codigo))
                {
                    codigosVeiculos.Add(acertoCarga.Carga.Veiculo.Codigo);

                    Dominio.Entidades.Embarcador.Acerto.AcertoVeiculoSegmento segmento = new Dominio.Entidades.Embarcador.Acerto.AcertoVeiculoSegmento()
                    {
                        AcertoViagem = acertoViagem,
                        Veiculo = acertoCarga.Carga.Veiculo,
                        ModeloVeicularCarga = acertoCarga.Carga.SegmentoModeloVeicularCarga,
                        GrupoPessoas = acertoCarga.Carga.SegmentoGrupoPessoas
                    };

                    repAcertoVeiculoSegmento.Inserir(segmento);
                }
            }

            Servicos.Log.GravarInfo(acertoViagem?.Codigo.ToString() + " Fim AtualizarVeiculosSegmento " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
        }

        public void DeletarResultadosAcertoViagem(Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Servicos.Log.GravarInfo(acertoViagem?.Codigo.ToString() + " Inicio DeletarResultadosAcertoViagem " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");

            Repositorio.Embarcador.Acerto.AcertoVeiculoResultado repAcertoVeiculoResultado = new Repositorio.Embarcador.Acerto.AcertoVeiculoResultado(unidadeDeTrabalho);
            List<Dominio.Entidades.Embarcador.Acerto.AcertoVeiculoResultado> listaResultado = repAcertoVeiculoResultado.BuscarPorAcerto(acertoViagem.Codigo);
            for (int i = 0; i < listaResultado.Count; i++)
            {
                listaResultado[i].VeiculosVinculados.Clear();
                repAcertoVeiculoResultado.Deletar(listaResultado[i]);
            }

            Servicos.Log.GravarInfo(acertoViagem?.Codigo.ToString() + " Fim DeletarResultadosAcertoViagem " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
        }

        public void InserirResultadosAcertoViagem(Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem, Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.Entidades.Usuario operador, bool naoLancarDescontosDasOcorrenciasNoAcertoDeViagem, bool acertoDeViagemImpressaoDetalhada, bool gerarTituloPagarFolhaFuncionario, bool gerarReciboAcertoViagemDetalhado, bool visualizarReciboPorMotoristaNoAcertoDeViagem)
        {
            Servicos.Log.GravarInfo(acertoViagem?.Codigo.ToString() + " Inicio InserirResultadosAcertoViagem " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");

            Repositorio.Embarcador.Acerto.AcertoVeiculoResultado repAcertoVeiculoResultado = new Repositorio.Embarcador.Acerto.AcertoVeiculoResultado(unidadeDeTrabalho);
            Repositorio.Embarcador.Acerto.AcertoVeiculoSegmento repAcertoVeiculoSegmento = new Repositorio.Embarcador.Acerto.AcertoVeiculoSegmento(unidadeDeTrabalho);
            Repositorio.Embarcador.Acerto.AcertoResumoAbastecimento repAcertoResumoAbastecimento = new Repositorio.Embarcador.Acerto.AcertoResumoAbastecimento(unidadeDeTrabalho);
            Repositorio.Embarcador.Acerto.AcertoAbastecimento repAcertoAbastecimento = new Repositorio.Embarcador.Acerto.AcertoAbastecimento(unidadeDeTrabalho);
            Repositorio.Embarcador.Acerto.AcertoCarga repAcertoCarga = new Repositorio.Embarcador.Acerto.AcertoCarga(unidadeDeTrabalho);
            Repositorio.Embarcador.Acerto.AcertoVeiculo repAcertoVeiculo = new Repositorio.Embarcador.Acerto.AcertoVeiculo(unidadeDeTrabalho);
            Repositorio.Embarcador.Acerto.AcertoCarga repCarga = new Repositorio.Embarcador.Acerto.AcertoCarga(unidadeDeTrabalho);
            Repositorio.Embarcador.Acerto.AcertoOutraDespesa repDespesa = new Repositorio.Embarcador.Acerto.AcertoOutraDespesa(unidadeDeTrabalho);
            Repositorio.Embarcador.Acerto.AcertoAdiantamento repAcertoAdiantamento = new Repositorio.Embarcador.Acerto.AcertoAdiantamento(unidadeDeTrabalho);
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS repPagamentoMotoristaTMS = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS(unidadeDeTrabalho);
            Repositorio.Embarcador.Configuracoes.ConfiguraoAcertoViagem repConfiguraoAcertoViagem = new Repositorio.Embarcador.Configuracoes.ConfiguraoAcertoViagem(unidadeDeTrabalho);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguraoAcertoViagem configuraoAcertoViagem = repConfiguraoAcertoViagem.BuscarConfiguracaoPadrao();
            List<Dominio.Entidades.Embarcador.Acerto.AcertoVeiculoSegmento> listaVeiculos = repAcertoVeiculoSegmento.BuscarPorAcerto(acertoViagem.Codigo);
            List<Dominio.Relatorios.Embarcador.DataSource.AcertoViagem.ReceitaAcertoViagem> receita = this.RetornaObjetoReceitaViagem(acertoViagem.Codigo, unidadeDeTrabalho, naoLancarDescontosDasOcorrenciasNoAcertoDeViagem, acertoDeViagemImpressaoDetalhada, gerarTituloPagarFolhaFuncionario, gerarReciboAcertoViagemDetalhado, visualizarReciboPorMotoristaNoAcertoDeViagem, (configuraoAcertoViagem?.SepararValoresAdiantamentoMotoristaPorTipo ?? false));

            for (int i = 0; i < listaVeiculos.Count; i++)
            {
                Dominio.Entidades.Embarcador.Acerto.AcertoResumoAbastecimento resumoAbastecimento = repAcertoResumoAbastecimento.BuscarPorCodigoAcertoVeiculoTipo(acertoViagem.Codigo, listaVeiculos[i].Veiculo.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Combustivel);
                Dominio.Entidades.Embarcador.Acerto.AcertoVeiculoResultado resultado = new Dominio.Entidades.Embarcador.Acerto.AcertoVeiculoResultado();
                resultado.AcertoViagem = acertoViagem;
                resultado.Tracao = listaVeiculos[i].Veiculo;
                resultado.KMTotal = resumoAbastecimento?.KMTotalAjustado ?? 0;
                resultado.ValorCombustivelTracao = repAcertoAbastecimento.ValorTotalAbastecimentos(acertoViagem.Codigo, listaVeiculos[i].Veiculo.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Combustivel);

                decimal impostosCarga = repCarga.ValorICMSAcerto(acertoViagem.Codigo, true, listaVeiculos[i].Veiculo.Codigo);
                impostosCarga += repCarga.ValorICMSFracionadoAcerto(acertoViagem.Codigo, listaVeiculos[i].Veiculo.Codigo);
                resultado.ValorICMS = impostosCarga;
                //resultado.ValorICMS = repAcertoCarga.ValorICMSCargasVeiculo(acertoViagem.Codigo, listaVeiculos[i].Veiculo.Codigo);

                if (resumoAbastecimento != null && resumoAbastecimento.Litros > 0)
                    resultado.ValorMediaTracao = Math.Round((resumoAbastecimento.KMFinal - resumoAbastecimento.KMInicial) / resumoAbastecimento.Litros, 2);//resumoAbastecimento.Media;
                else
                    resultado.ValorMediaTracao = 0;
                resultado.ValorParametroMediaTracao = resumoAbastecimento?.MediaIdeal ?? 0;

                resultado.DespesaCombustivel = acertoViagem.Abastecimentos != null ? (from obj in acertoViagem.Abastecimentos where obj.Abastecimento.Veiculo.Codigo == listaVeiculos[i].Veiculo.Codigo && obj.Abastecimento.TipoAbastecimento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Combustivel select obj.Abastecimento.ValorTotal).Sum() : 0;
                resultado.DespesaArla = acertoViagem.Abastecimentos != null ? (from obj in acertoViagem.Abastecimentos where obj.Abastecimento.Veiculo.Codigo == listaVeiculos[i].Veiculo.Codigo && obj.Abastecimento.TipoAbastecimento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Arla select obj.Abastecimento.ValorTotal).Sum() : 0;
                resultado.DespesaPedagio = acertoViagem.Pedagios != null ? (from obj in acertoViagem.Pedagios where obj.Pedagio.TipoPedagio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedagio.Debito && obj.Pedagio.Veiculo.Codigo == listaVeiculos[i].Veiculo.Codigo select obj.Pedagio.Valor).Sum() : 0;
                resultado.DespesaMotorista = acertoViagem.OutrasDespesas != null ? (from obj in acertoViagem.OutrasDespesas where obj.Veiculo.Codigo == listaVeiculos[i].Veiculo.Codigo && obj.Quantidade > 0 select obj.Valor * obj.Quantidade).Sum() : 0;
                resultado.DespesaMotorista += acertoViagem.OutrasDespesas != null ? (from obj in acertoViagem.OutrasDespesas where obj.Veiculo.Codigo == listaVeiculos[i].Veiculo.Codigo && obj.Quantidade == 0 select obj.Valor).Sum() : 0;
                resultado.Ocorrencias = acertoViagem.Ocorrencias != null ? (from obj in acertoViagem.Ocorrencias select obj.CargaOcorrencia.ValorOcorrencia).Sum() : 0;

                resultado.AbastecimentoMotorista = acertoViagem.Abastecimentos != null ? (from obj in acertoViagem.Abastecimentos where obj.Abastecimento.Posto.Modalidades.Any(o => o.TipoModalidade == TipoModalidade.Fornecedor && o.ModalidadesFornecedores.Any(p => !p.PagoPorFatura)) select obj.Abastecimento.ValorTotal).Sum() : 0;
                resultado.PedagioMotorista = acertoViagem.Pedagios != null ? (from obj in acertoViagem.Pedagios where obj.Pedagio.TipoPedagio == TipoPedagio.Debito && !obj.Pedagio.ImportadoDeSemParar select obj.Pedagio.Valor).Sum() : 0;
                resultado.OutraDespesaMotorista = acertoViagem.OutrasDespesas != null ? (from obj in acertoViagem.OutrasDespesas where obj.TipoPagamento == TipoPagamentoAcertoDespesa.Motorista || (obj.TipoPagamento != TipoPagamentoAcertoDespesa.Empresa && obj.Pessoa.Modalidades.Any(o => o.TipoModalidade == TipoModalidade.Fornecedor && o.ModalidadesFornecedores.Any(p => !p.PagoPorFatura))) select obj.Valor * (obj.Quantidade > 0 ? obj.Quantidade : 1)).Sum() : 0;
                //resultado.AdiantamentoMotorista = repPagamentoMotoristaTMS.BuscarAdiantamentoMotorista(acertoViagem.DataInicial, acertoViagem.DataFinal.HasValue ? acertoViagem.DataFinal.Value : DateTime.Now.Date, acertoViagem.Motorista.Codigo);
                resultado.AdiantamentoMotorista = repAcertoAdiantamento.BuscarValorTotalPorAcerto(acertoViagem.Codigo);
                resultado.DiariaMotorista = acertoViagem.Diarias != null ? (from obj in acertoViagem.Diarias select obj.Valor).Sum() : 0;
                resultado.SaldoMotorista = resultado.AdiantamentoMotorista - (resultado.DiariaMotorista + resultado.AbastecimentoMotorista + resultado.PedagioMotorista + resultado.OutraDespesaMotorista);
                resultado.OperadorFechamento = operador;
                resultado.PlanoContaUsuario = operador.PlanoConta;

                decimal valorComponenteSubtrair = repCarga.ValorComponenteFretePorTipoAcerto(acertoViagem.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoAcertoViagem.Subtrair, false);
                decimal valorComponenteSomar = repCarga.ValorComponenteFretePorTipoAcerto(acertoViagem.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoAcertoViagem.Somar, false);

                decimal freteBruto = acertoViagem.Cargas != null ? (from obj in acertoViagem.Cargas where obj.Carga.Veiculo != null && obj.Carga.Veiculo.Codigo == listaVeiculos[i].Veiculo.Codigo select obj.ValorBrutoCarga).Sum() : 0;
                decimal valorComponentePedagio = repCarga.ValorComponenteFreteVeiculo(acertoViagem.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PEDAGIO, listaVeiculos[i].Veiculo.Codigo);
                decimal impostos = acertoViagem.Cargas != null ? (from obj in acertoViagem.Cargas where obj.Carga.Veiculo != null && obj.Carga.Veiculo.Codigo == listaVeiculos[i].Veiculo.Codigo select obj.ValorICMSCarga).Sum() : 0;
                decimal valorFreteLiquido = repCarga.ValorFreteLiquido(acertoViagem.Codigo, listaVeiculos[i].Veiculo.Codigo);
                decimal valorCTeComplementar = repCarga.ValorFreteLiquidoComplementar(acertoViagem.Codigo, 0);
                valorFreteLiquido += repCarga.ValorFreteLiquidoFracionadoAcerto(acertoViagem.Codigo);

                resultado.ReceitaFrete = valorFreteLiquido;// freteBruto - impostos - valorComponentePedagio;

                freteBruto = acertoViagem.Cargas != null ? (from obj in acertoViagem.Cargas where obj.Carga.Veiculo != null && obj.Carga.Veiculo.Codigo == listaVeiculos[i].Veiculo.Codigo select obj.ValorBrutoCarga).Sum() : 0;
                valorComponentePedagio = repCarga.ValorComponenteFreteVeiculo(acertoViagem.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PEDAGIO, listaVeiculos[i].Veiculo.Codigo);
                impostos = acertoViagem.Cargas != null ? (from obj in acertoViagem.Cargas where obj.Carga.Veiculo != null && obj.Carga.Veiculo.Codigo == listaVeiculos[i].Veiculo.Codigo select obj.ValorICMSCarga).Sum() : 0;
                resultado.ValorFaturamentoBruto = valorFreteLiquido + valorCTeComplementar;// freteBruto - impostos - valorComponenteSubtrair - valorComponentePedagio;

                decimal receitaPedagio = acertoViagem.Cargas != null ? (from obj in acertoViagem.Cargas where obj.Carga.Veiculo != null && obj.Carga.Veiculo.Codigo == listaVeiculos[i].Veiculo.Codigo select obj.PedagioAcerto).Sum() : 0;
                receitaPedagio += acertoViagem.Cargas != null ? (from obj in acertoViagem.Cargas where obj.Carga.Veiculo != null && obj.Carga.Veiculo.Codigo == listaVeiculos[i].Veiculo.Codigo select obj.PedagioAcertoCredito).Sum() : 0;
                receitaPedagio += acertoViagem.Pedagios != null ? (from obj in acertoViagem.Pedagios where obj.Pedagio.TipoPedagio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedagio.Credito && obj.Pedagio.Veiculo.Codigo == listaVeiculos[i].Veiculo.Codigo select obj.Pedagio.Valor).Sum() : 0;

                resultado.ReceitaPedagio = valorComponentePedagio + receitaPedagio;
                resultado.ReceitaOutros = valorComponenteSomar - valorComponenteSubtrair;
                resultado.ReceitaBonificacao = acertoViagem.Cargas != null ? (from obj in acertoViagem.Cargas where obj.Carga.Veiculo != null && obj.Carga.Veiculo.Codigo == listaVeiculos[i].Veiculo.Codigo select obj.ValorBonificacaoCliente).Sum() : 0;

                if (acertoViagem.DataFinal.HasValue)
                    resultado.NumeroViagens = (int)(acertoViagem.DataFinal.Value - acertoViagem.DataInicial).TotalDays;
                else
                    resultado.NumeroViagens = 1;

                resultado.QuantidadeViagemCompartilhada = acertoViagem.Cargas != null ? (from obj in acertoViagem.Cargas where obj.Carga.Veiculo != null && obj.Carga.Veiculo.Codigo == listaVeiculos[i].Veiculo.Codigo && obj.PercentualAcerto < 100 select obj).Count() : 0;
                resultado.ValorViagemCompartilhada = acertoViagem.Cargas != null ? (from obj in acertoViagem.Cargas where obj.Carga.Veiculo != null && obj.Carga.Veiculo.Codigo == listaVeiculos[i].Veiculo.Codigo && obj.PercentualAcerto < 100 select obj.ValorBrutoCarga).Sum() : 0;
                resultado.DespesaMotorista = 0;
                resultado.DespesaMotorista = repDespesa.ReceitaDespesaOutraDespesaVeiculo(acertoViagem.Codigo, false, listaVeiculos[i].Veiculo.Codigo);
                if (acertoViagem.Motorista.NaoGeraComissaoAcerto)
                    resultado.PercentualComissao = 0;
                else
                    resultado.PercentualComissao = receita[0].PercentualComissao;
                resultado.Bonificacao = acertoViagem.Bonificacoes != null ? (from obj in acertoViagem.Bonificacoes where obj.Veiculo != null && obj.Veiculo.Codigo == listaVeiculos[i].Veiculo.Codigo select obj.ValorBonificacao).Sum() : 0;
                resultado.Desconto = acertoViagem.Descontos != null ? (from obj in acertoViagem.Descontos where obj.Veiculo != null && obj.Veiculo.Codigo == listaVeiculos[i].Veiculo.Codigo select obj.ValorDesconto).Sum() : 0;
                resultado.SaldoMotorista = resultado.SaldoMotorista + resultado.Desconto;
                resultado.SaldoMotorista = resultado.SaldoMotorista - resultado.Bonificacao;

                List<Dominio.Entidades.Veiculo> listaReboque = repAcertoVeiculo.BuscarVeiculosVinculadosAcertoCargaVeiculo(acertoViagem.Codigo, listaVeiculos[i].Veiculo.Codigo);
                resultado.ValorCombustivelReboques = 0;

                for (int a = 0; a < listaReboque.Count; a++)
                {
                    resultado.VeiculosVinculados = new List<Dominio.Entidades.Veiculo>();
                    resultado.VeiculosVinculados.Add(listaReboque[a]);
                    //este não pega somente dos reboques
                    resultado.ValorCombustivelReboques += repAcertoAbastecimento.ValorTotalAbastecimentos(acertoViagem.Codigo, listaReboque[a].Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Combustivel);
                    resultado.ValorCombustivelReboques += repAcertoAbastecimento.ValorTotalAbastecimentos(acertoViagem.Codigo, listaReboque[a].Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Arla);
                    resultado.ValorCombustivelReboques += repAcertoAbastecimento.ValorTotalAbastecimentosEquipamento(acertoViagem.Codigo, listaReboque[a].Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Combustivel);
                    resultado.ValorCombustivelReboques += repAcertoAbastecimento.ValorTotalAbastecimentosEquipamento(acertoViagem.Codigo, listaReboque[a].Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Arla);

                    resultado.DespesaCombustivel += acertoViagem.Abastecimentos != null ? (from obj in acertoViagem.Abastecimentos where obj.Abastecimento.Veiculo.Codigo == listaReboque[a].Codigo && obj.Abastecimento.TipoAbastecimento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Combustivel select obj.Abastecimento.ValorTotal).Sum() : 0;
                    resultado.DespesaArla += acertoViagem.Abastecimentos != null ? (from obj in acertoViagem.Abastecimentos where obj.Abastecimento.Veiculo.Codigo == listaReboque[a].Codigo && obj.Abastecimento.TipoAbastecimento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Arla select obj.Abastecimento.ValorTotal).Sum() : 0;
                    resultado.DespesaPedagio += acertoViagem.Pedagios != null ? (from obj in acertoViagem.Pedagios where obj.Pedagio.TipoPedagio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedagio.Debito && obj.Pedagio.Veiculo.Codigo == listaReboque[a].Codigo select obj.Pedagio.Valor).Sum() : 0;
                    resultado.DespesaMotorista += acertoViagem.OutrasDespesas != null ? (from obj in acertoViagem.OutrasDespesas where obj.Veiculo.Codigo == listaReboque[a].Codigo && obj.Quantidade > 0 select obj.Valor * obj.Quantidade).Sum() : 0;
                    resultado.DespesaMotorista += acertoViagem.OutrasDespesas != null ? (from obj in acertoViagem.OutrasDespesas where obj.Veiculo.Codigo == listaReboque[a].Codigo && obj.Quantidade == 0 select obj.Valor).Sum() : 0;

                    freteBruto = acertoViagem.Cargas != null ? (from obj in acertoViagem.Cargas where obj.Carga.Veiculo != null && obj.Carga.Veiculo.Codigo == listaReboque[a].Codigo select obj.ValorBrutoCarga).Sum() : 0;
                    valorComponentePedagio = repCarga.ValorComponenteFreteVeiculo(acertoViagem.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PEDAGIO, listaReboque[a].Codigo);
                    valorFreteLiquido = repCarga.ValorFreteLiquido(acertoViagem.Codigo, listaReboque[a].Codigo);
                    valorFreteLiquido += repCarga.ValorFreteLiquidoFracionadoAcerto(acertoViagem.Codigo);
                    valorComponenteSubtrair = repCarga.ValorComponenteFretePorTipoAcerto(acertoViagem.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoAcertoViagem.Subtrair, false);

                    impostos = acertoViagem.Cargas != null ? (from obj in acertoViagem.Cargas where obj.Carga.Veiculo != null && obj.Carga.Veiculo.Codigo == listaReboque[a].Codigo select obj.ValorICMSCarga).Sum() : 0;
                    resultado.ReceitaFrete += valorFreteLiquido;// freteBruto - impostos - valorComponentePedagio;
                    receitaPedagio = acertoViagem.Cargas != null ? (from obj in acertoViagem.Cargas where obj.Carga.Veiculo != null && obj.Carga.Veiculo.Codigo == listaReboque[a].Codigo select obj.PedagioAcerto).Sum() : 0;
                    receitaPedagio += acertoViagem.Cargas != null ? (from obj in acertoViagem.Cargas where obj.Carga.Veiculo != null && obj.Carga.Veiculo.Codigo == listaReboque[a].Codigo select obj.PedagioAcertoCredito).Sum() : 0;
                    receitaPedagio += acertoViagem.Pedagios != null ? (from obj in acertoViagem.Pedagios where obj.Pedagio.TipoPedagio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedagio.Credito && obj.Pedagio.Veiculo.Codigo == listaReboque[a].Codigo select obj.Pedagio.Valor).Sum() : 0;

                    resultado.ReceitaPedagio += valorComponentePedagio + receitaPedagio;
                    resultado.ReceitaOutros += repCarga.ValorComponenteFreteVeiculo(acertoViagem.Codigo, listaReboque[a].Codigo) - valorComponenteSubtrair;
                    resultado.ReceitaBonificacao += acertoViagem.Cargas != null ? (from obj in acertoViagem.Cargas where obj.Carga.Veiculo != null && obj.Carga.Veiculo.Codigo == listaReboque[a].Codigo select obj.ValorBonificacaoCliente).Sum() : 0;
                    resultado.QuantidadeViagemCompartilhada += acertoViagem.Cargas != null ? (from obj in acertoViagem.Cargas where obj.Carga.Veiculo != null && obj.Carga.Veiculo.Codigo == listaReboque[a].Codigo && obj.PercentualAcerto < 100 select obj).Count() : 0;
                    resultado.ValorViagemCompartilhada += acertoViagem.Cargas != null ? (from obj in acertoViagem.Cargas where obj.Carga.Veiculo != null && obj.Carga.Veiculo.Codigo == listaReboque[a].Codigo && obj.PercentualAcerto < 100 select obj.ValorBrutoCarga).Sum() : 0;

                    resultado.Bonificacao += acertoViagem.Bonificacoes != null ? (from obj in acertoViagem.Bonificacoes where obj.Veiculo.Codigo == listaReboque[a].Codigo select obj.ValorBonificacao).Sum() : 0;
                    resultado.Desconto += acertoViagem.Descontos != null ? (from obj in acertoViagem.Descontos where obj.Veiculo.Codigo == listaReboque[a].Codigo select obj.ValorDesconto).Sum() : 0;
                }

                resultado.FreteLiquido = resultado.ReceitaFrete;
                if (acertoViagem.Motorista.NaoGeraComissaoAcerto)
                    resultado.ValorComissao = 0;
                else
                    resultado.ValorComissao = (resultado.FreteLiquido * resultado.PercentualComissao) / 100;

                resultado.TotalPagarMotorista = resultado.Bonificacao + resultado.ValorComissao + resultado.DespesaMotorista - resultado.Desconto;
                resultado.DespesaTotal = resultado.DespesaCombustivel + resultado.DespesaArla + resultado.DespesaPedagio + resultado.DespesaMotorista + resultado.ValorComissao + resultado.Bonificacao;
                resultado.ReceitaTotal = resultado.ReceitaFrete + resultado.ReceitaPedagio + resultado.ReceitaOutros + resultado.ReceitaBonificacao;

                resultado.SaldoFinal = receita[0].TotalSaldo;
                resultado.LiquidoMes = receita[0].ValorLiquidoMes;
                resultado.ValorResultadoLiquido = resultado.SaldoFinal - resultado.ValorComissao - resultado.Bonificacao;

                repAcertoVeiculoResultado.Inserir(resultado);
            }

            Servicos.Log.GravarInfo(acertoViagem?.Codigo.ToString() + " Fim InserirResultadosAcertoViagem " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
        }

        public void InserirOcorrenciaAcerto(Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem, Repositorio.UnitOfWork unidadeDeTrabalho, List<Dominio.Entidades.Veiculo> listaVeiculo, Dominio.Entidades.Usuario motorista)
        {
            Servicos.Log.GravarInfo(acertoViagem?.Codigo.ToString() + " Inicio InserirOcorrenciaAcerto " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");

            if (listaVeiculo.Count() == 0)
            {
                Servicos.Log.GravarInfo(acertoViagem?.Codigo.ToString() + " Fim InserirOcorrenciaAcerto " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                return;
            }

            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unidadeDeTrabalho);
            Repositorio.Embarcador.Acerto.AcertoOcorrencia repAcertoOcorrencia = new Repositorio.Embarcador.Acerto.AcertoOcorrencia(unidadeDeTrabalho);

            for (int i = 0; i < listaVeiculo.Count(); i++)
            {
                List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> listaOcorrencia = repCargaOcorrencia.ConsultarOcorrenciaSemAcerto(acertoViagem.DataInicial, acertoViagem.DataFinal.HasValue ? acertoViagem.DataFinal.Value : DateTime.Now.Date, motorista, listaVeiculo[i].Codigo);
                for (int a = 0; a < listaOcorrencia.Count(); a++)
                {
                    Dominio.Entidades.Embarcador.Acerto.AcertoOcorrencia ocorrencia = new Dominio.Entidades.Embarcador.Acerto.AcertoOcorrencia();
                    ocorrencia.AcertoViagem = acertoViagem;
                    ocorrencia.CargaOcorrencia = listaOcorrencia[a];
                    ocorrencia.LancadoManualmente = false;

                    repAcertoOcorrencia.Inserir(ocorrencia);

                }

            }

            Servicos.Log.GravarInfo(acertoViagem?.Codigo.ToString() + " Fim InserirOcorrenciaAcerto " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");

        }

        public void InserirAdiantamentoAcerto(Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem, Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.Entidades.Usuario motorista)
        {
            Servicos.Log.GravarInfo(acertoViagem?.Codigo.ToString() + " Inicio InserirAdiantamentoAcerto " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");

            if (motorista == null)
            {
                Servicos.Log.GravarInfo(acertoViagem?.Codigo.ToString() + " Fim InserirAdiantamentoAcerto " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                return;
            }

            Repositorio.Embarcador.Acerto.AcertoAdiantamento repAcertoAdiantamento = new Repositorio.Embarcador.Acerto.AcertoAdiantamento(unidadeDeTrabalho);
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS repPagamentoMotoristaTMS = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS(unidadeDeTrabalho);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            List<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS> adiantamentos = repPagamentoMotoristaTMS.BuscarAdiantamentosMotorista(true, configuracaoTMS.BuscarAdiantamentosSemDataInicialAcertoViagem ? DateTime.MinValue : acertoViagem.DataInicial, acertoViagem.DataFinal, motorista.Codigo, acertoViagem.Codigo);
            foreach (var adiant in adiantamentos)
            {
                int codigoAdiantamento = (int)adiant.Codigo;

                Dominio.Entidades.Embarcador.Acerto.AcertoAdiantamento adi = new Dominio.Entidades.Embarcador.Acerto.AcertoAdiantamento();
                adi.AcertoViagem = acertoViagem;
                adi.PagamentoMotoristaTMS = repPagamentoMotoristaTMS.BuscarPorCodigo(codigoAdiantamento);
                repAcertoAdiantamento.Inserir(adi);
            }

            Servicos.Log.GravarInfo(acertoViagem?.Codigo.ToString() + " Fim InserirAdiantamentoAcerto " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");

        }

        public void InserirDiariasAcerto(Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem, Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.Entidades.Usuario motorista)
        {
            Servicos.Log.GravarInfo(acertoViagem?.Codigo.ToString() + " Inicio InserirDiariasAcerto " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");

            if (motorista == null)
            {
                Servicos.Log.GravarInfo(acertoViagem?.Codigo.ToString() + " Fim InserirDiariasAcerto " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                return;
            }

            Repositorio.Embarcador.Acerto.AcertoAdiantamento repAcertoAdiantamento = new Repositorio.Embarcador.Acerto.AcertoAdiantamento(unidadeDeTrabalho);
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS repPagamentoMotoristaTMS = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS(unidadeDeTrabalho);

            List<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS> adiantamentos = repPagamentoMotoristaTMS.BuscarDiariasMotorista(acertoViagem.DataInicial, acertoViagem.DataFinal, motorista.Codigo, acertoViagem.Codigo);
            foreach (var adiant in adiantamentos)
            {
                int codigoAdiantamento = (int)adiant.Codigo;

                Dominio.Entidades.Embarcador.Acerto.AcertoAdiantamento adi = new Dominio.Entidades.Embarcador.Acerto.AcertoAdiantamento();
                adi.AcertoViagem = acertoViagem;
                adi.PagamentoMotoristaTMS = repPagamentoMotoristaTMS.BuscarPorCodigo(codigoAdiantamento);
                repAcertoAdiantamento.Inserir(adi);
            }

            Servicos.Log.GravarInfo(acertoViagem?.Codigo.ToString() + " Fim InserirAdiantamentoAcerto " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");

        }

        public void InserirInfracoesAcerto(Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem, Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.Entidades.Usuario motorista)
        {
            Servicos.Log.GravarInfo(acertoViagem?.Codigo.ToString() + " Inicio InserirInfracoesAcerto " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");

            if (motorista == null)
            {
                Servicos.Log.GravarInfo(acertoViagem?.Codigo.ToString() + " Fim InserirInfracoesAcerto " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");
                return;
            }

            Repositorio.Embarcador.Acerto.AcertoViagemInfracao repAcertoViagemInfracao = new Repositorio.Embarcador.Acerto.AcertoViagemInfracao(unidadeDeTrabalho);
            Repositorio.Embarcador.Frota.Infracao repInfracao = new Repositorio.Embarcador.Frota.Infracao(unidadeDeTrabalho);
            Repositorio.Embarcador.Acerto.AcertoDesconto repAcertoDesconto = new Repositorio.Embarcador.Acerto.AcertoDesconto(unidadeDeTrabalho);

            List<Dominio.Entidades.Embarcador.Frota.Infracao> infracoes = repInfracao.BuscarInfracoesMotorista(true, null, null, motorista.Codigo, acertoViagem.Codigo);
            foreach (var infracao in infracoes)
            {
                int codigoInfracao = (int)infracao.Codigo;

                Dominio.Entidades.Embarcador.Acerto.AcertoViagemInfracao adi = new Dominio.Entidades.Embarcador.Acerto.AcertoViagemInfracao();
                adi.AcertoViagem = acertoViagem;
                adi.Infracao = repInfracao.BuscarPorCodigo(codigoInfracao);
                adi.InfracaoAssinada = false;
                repAcertoViagemInfracao.Inserir(adi);

                if (infracao.LancarDescontoMotorista && infracao.DescontoComissaoMotorista > 0 && infracao.JustificativaDesconto != null)
                {
                    Dominio.Entidades.Embarcador.Acerto.AcertoDesconto descontoTabela = new Dominio.Entidades.Embarcador.Acerto.AcertoDesconto()
                    {
                        AcertoViagem = acertoViagem,
                        Data = acertoViagem.DataFinal.HasValue ? acertoViagem.DataFinal.Value : acertoViagem.DataInicial,
                        Justificativa = infracao.JustificativaDesconto,
                        Motivo = "DESCONTO LANÇADO DEVIDO OCORRÊNCIA AO MOTORISTA Nº " + infracao.Numero.ToString(),
                        ValorDesconto = infracao.DescontoComissaoMotorista,
                        Veiculo = infracao.Veiculo
                    };
                    repAcertoDesconto.Inserir(descontoTabela);
                }
            }

            Servicos.Log.GravarInfo(acertoViagem?.Codigo.ToString() + " Fim InserirInfracoesAcerto " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "Log Acerto Viagem");

        }

        public bool GerarTituloMotoristaSaldo(ref string msgRetorno, Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem, Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.Entidades.Usuario usuario, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string stringConexao, bool exibirSaldoPrevistoAcertoViagem, bool visualizarReciboPorMotoristaNoAcertoDeViagem, decimal saldoViagemMotorista, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, decimal valorSaldoMotorista)
        {
            msgRetorno = string.Empty;
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unidadeDeTrabalho);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);
            Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unidadeDeTrabalho);
            Servicos.Embarcador.Financeiro.ProcessoMovimento svcProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(stringConexao);

            if (!acertoViagem.DataVencimentoMotoristaAcerto.HasValue || acertoViagem.TipoMovimentoMotoristaAcerto == null)
            {
                msgRetorno = "Favor informe a data de vencimento e o tipo de movimento na forma de recebimento do motorista.";
                return false;
            }
            if (acertoViagem.DataVencimentoMotoristaAcerto.Value < DateTime.Now.Date)
            {
                msgRetorno = "A data de vencimento não pode ser menor que a data atual.";
                return false;
            }

            Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = new Dominio.Entidades.Embarcador.Financeiro.Titulo();

            DateTime? dataVencimento = acertoViagem.DataVencimentoMotoristaAcerto.Value;

            Dominio.Entidades.Cliente pessoa = repCliente.BuscarPorCPFCNPJ(double.Parse(acertoViagem.Motorista.CPF));
            if (pessoa == null)
            {
                if (acertoViagem.Motorista.Localidade == null)
                {
                    msgRetorno = "Motorista está com o endereço incompleto, favor ajustar antes de prosseguir.";
                    return false;
                }

                pessoa = Servicos.Embarcador.Pessoa.Pessoa.ConverterFuncionario(acertoViagem.Motorista, unidadeDeTrabalho);
                repCliente.Inserir(pessoa);
            }

            saldoViagemMotorista = Math.Round((saldoViagemMotorista * -1), 2, MidpointRounding.ToEven);

            titulo.TipoTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Receber;
            titulo.DataEmissao = DateTime.Now;
            titulo.DataVencimento = dataVencimento;
            titulo.DataProgramacaoPagamento = dataVencimento;
            titulo.Pessoa = pessoa;
            titulo.GrupoPessoas = titulo.Pessoa.GrupoPessoas;
            titulo.Sequencia = 1;
            titulo.ValorOriginal = saldoViagemMotorista;
            titulo.ValorPendente = saldoViagemMotorista;
            titulo.StatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto;
            titulo.DataAlteracao = DateTime.Now;
            titulo.Observacao = "Referente ao saldo do fechamento do acerto de viagem " + acertoViagem.Numero.ToString("D");
            titulo.Empresa = null;
            titulo.ValorTituloOriginal = titulo.ValorOriginal;
            titulo.TipoDocumentoTituloOriginal = "Acerto de Viagem";
            titulo.NumeroDocumentoTituloOriginal = acertoViagem.Numero.ToString("D");
            titulo.FormaTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo.Outros;
            titulo.TipoMovimento = acertoViagem.TipoMovimentoMotoristaAcerto;
            titulo.Usuario = acertoViagem.Operador;
            titulo.DataLancamento = DateTime.Now;
            titulo.AcertoViagem = acertoViagem;

            repTitulo.Inserir(titulo);

            if (!svcProcessoMovimento.GerarMovimentacao(out msgRetorno, titulo.TipoMovimento, titulo.DataEmissao.Value, titulo.ValorOriginal, acertoViagem.Numero.ToString("D"), titulo.Observacao, unidadeDeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Outros, tipoServicoMultisoftware, 0, null, null, titulo.Codigo, null, null, null, titulo.DataEmissao.Value))
                return false;

            if (!this.GerarControleSaldoMotorista(ref msgRetorno, acertoViagem, unidadeDeTrabalho, usuario, auditado, tipoServicoMultisoftware, stringConexao, configuracaoTMS.ExibirSaldoPrevistoAcertoViagem, configuracaoTMS.VisualizarReciboPorMotoristaNoAcertoDeViagem, (valorSaldoMotorista * -1)))
                return false;

            acertoViagem.ValorMovimentadoFichaMotorista = (valorSaldoMotorista * -1);
            repAcertoViagem.Atualizar(acertoViagem);

            return true;
        }

        public bool GerarPagamentoMotoristaSaldoMotorista(ref string msgRetorno, Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem, Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.Entidades.Usuario usuario, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string stringConexao, bool exibirSaldoPrevistoAcertoViagem, bool visualizarReciboPorMotoristaNoAcertoDeViagem, decimal saldoViagemMotorista, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS)
        {
            msgRetorno = string.Empty;

            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo repPagamentoMotoristaTipo = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo(unidadeDeTrabalho);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unidadeDeTrabalho);
            Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unidadeDeTrabalho);
            Repositorio.Embarcador.Configuracoes.ConfiguraoAcertoViagem repConfiguraoAcertoViagem = new Repositorio.Embarcador.Configuracoes.ConfiguraoAcertoViagem(unidadeDeTrabalho);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguraoAcertoViagem configuraoAcertoViagem = repConfiguraoAcertoViagem.BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo pagamentoMotoristaTipo = repPagamentoMotoristaTipo.BuscarPrimeiroParaGeracaoSaldo();
            if (pagamentoMotoristaTipo == null)
                return true;

            Dominio.Entidades.Usuario motorista = repUsuario.BuscarPorCodigo(acertoViagem.Motorista.Codigo, true);

            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS repPagamentoMotorista = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS(unidadeDeTrabalho);
            if (repPagamentoMotorista.ContemPorAcerto(acertoViagem.Codigo, motorista.Codigo))
            {
                msgRetorno = "Já existe um pagamento lançado para este acerto, favor cancele ou rejeite o mesmo para seguir com o fechamento deste.";
                return false;
            }

            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio repPagamentoMotoristaIntegracaoEnvio = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio(unidadeDeTrabalho);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);

            Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamentoMotorista = new Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS();

            pagamentoMotorista.Usuario = usuario;
            pagamentoMotorista.SituacaoPagamentoMotorista = SituacaoPagamentoMotorista.AgInformacoes;
            pagamentoMotorista.Data = DateTime.Now.Date;
            pagamentoMotorista.EtapaPagamentoMotorista = EtapaPagamentoMotorista.Iniciada;
            pagamentoMotorista.Numero = repPagamentoMotorista.BuscarProximoNumero();
            pagamentoMotorista.PagamentoLiberado = true;

            pagamentoMotorista.Carga = null;
            pagamentoMotorista.AcertoViagem = acertoViagem;
            pagamentoMotorista.Motorista = motorista;
            pagamentoMotorista.PagamentoMotoristaTipo = pagamentoMotoristaTipo;
            pagamentoMotorista.DataPagamento = DateTime.Now;
            pagamentoMotorista.DataVencimentoTituloPagar = pagamentoMotorista.DataPagamento;
            pagamentoMotorista.Valor = saldoViagemMotorista;
            pagamentoMotorista.Observacao = $"PAGAMENTO REFERENTE SALDO ACERTO NÚMERO { acertoViagem.Numero } ";
            if (!string.IsNullOrWhiteSpace(acertoViagem.Observacao))
                pagamentoMotorista.Observacao += " " + acertoViagem.Observacao;
            pagamentoMotorista.PessoaTituloPagar = null;

            if (pagamentoMotorista.Motorista.PlanoAcertoViagem != null && usuario.PlanoConta != null)
            {
                pagamentoMotorista.PlanoDeContaDebito = usuario.PlanoConta;
                pagamentoMotorista.PlanoDeContaCredito = pagamentoMotorista.Motorista.PlanoAcertoViagem;
            }
            else if (pagamentoMotorista.PagamentoMotoristaTipo.GerarMovimentoAutomatico && pagamentoMotorista.PagamentoMotoristaTipo.TipoMovimentoLancamento != null)
            {
                pagamentoMotorista.PlanoDeContaDebito = pagamentoMotorista.PagamentoMotoristaTipo.TipoMovimentoLancamento.PlanoDeContaDebito;
                pagamentoMotorista.PlanoDeContaCredito = pagamentoMotorista.PagamentoMotoristaTipo.TipoMovimentoLancamento.PlanoDeContaCredito;
            }

            if (pagamentoMotorista.PagamentoMotoristaTipo != null && pagamentoMotorista.PagamentoMotoristaTipo.GerarMovimentoAutomatico && (pagamentoMotorista.PlanoDeContaCredito == null || pagamentoMotorista.PlanoDeContaDebito == null))
            {
                msgRetorno = "Favor selecione os Planos de Contas para este Tipo de Pagamento.";
                return false;
            }

            if (repPagamentoMotorista.ContemPagamentoEmAberto(pagamentoMotorista.Motorista.Codigo))
            {
                msgRetorno = ("Já existe um pagamento em aberto para este motorista, favor finalize o mesmo antes de iniciar um novo.");
                return false;
            }

            if (repPagamentoMotorista.ContemPagamentoIdentico(pagamentoMotorista.DataPagamento.Date, pagamentoMotorista.Motorista.Codigo, pagamentoMotorista.PagamentoMotoristaTipo.Codigo, pagamentoMotorista.Valor))
            {
                msgRetorno = ("Já existe um pagamento com a mesma Data do Pagamento, Motorista, Tipo do Pagamento e Valor.");
                return false;
            }

            Servicos.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS.CalcularImpostos(ref pagamentoMotorista, unidadeDeTrabalho, tipoServicoMultisoftware);

            repPagamentoMotorista.Inserir(pagamentoMotorista, auditado);

            TipoIntegracaoPagamentoMotorista tipoIntegracaoPagamentoMotorista = pagamentoMotorista.PagamentoMotoristaTipo?.TipoIntegracaoPagamentoMotorista ?? TipoIntegracaoPagamentoMotorista.SemIntegracao;

            if (Servicos.Embarcador.PagamentoMotorista.AutorizacaoPagamentoMotorista.VerificarRegrasAutorizacaoPagamentoMotorista(pagamentoMotorista, tipoServicoMultisoftware, unidadeDeTrabalho, usuario, unidadeDeTrabalho.StringConexao, auditado, out bool contemAprovadorIgualAoOperador))
            {
                pagamentoMotorista.SituacaoPagamentoMotorista = SituacaoPagamentoMotorista.AutorizacaoPendente;
                pagamentoMotorista.EtapaPagamentoMotorista = EtapaPagamentoMotorista.AgAutorizacao;
            }
            else
            {
                pagamentoMotorista.SituacaoPagamentoMotorista = SituacaoPagamentoMotorista.AgIntegracao;
                pagamentoMotorista.EtapaPagamentoMotorista = EtapaPagamentoMotorista.Integracao;
            }

            if (contemAprovadorIgualAoOperador)
            {
                Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao repPagamentoMotoristaAutorizacao = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao pagamentoMotoristaAutorizacao = repPagamentoMotoristaAutorizacao.BuscarPrimeiroPorPagamentoUsuario(pagamentoMotorista.Codigo, pagamentoMotorista.Usuario.Codigo);

                Servicos.Embarcador.PagamentoMotorista.AutorizacaoPagamentoMotorista.EfetuarAprovacao(pagamentoMotoristaAutorizacao, pagamentoMotorista.Usuario, unidadeDeTrabalho, unidadeDeTrabalho.StringConexao, tipoServicoMultisoftware, configuracaoTMS);

                msgRetorno = "";
                Servicos.Embarcador.PagamentoMotorista.AutorizacaoPagamentoMotorista.VerificarSituacaoPagamento(pagamentoMotoristaAutorizacao.PagamentoMotoristaTMS, unidadeDeTrabalho, ref msgRetorno, tipoServicoMultisoftware, auditado, unidadeDeTrabalho.StringConexao, configuracaoTMS, pagamentoMotorista.Usuario);
                Servicos.Auditoria.Auditoria.Auditar(auditado, pagamentoMotorista, null, "Aprovou o pagamento pelo mesmo operadora da alçada.", unidadeDeTrabalho);
            }

            Repositorio.Embarcador.Configuracoes.IntegracaoKMM repositorioIntegracaoKMM = new Repositorio.Embarcador.Configuracoes.IntegracaoKMM(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKMM configuracaoIntegracaoKMM = repositorioIntegracaoKMM.BuscarPrimeiroRegistro();

            if (tipoIntegracaoPagamentoMotorista.PossuiIntegracao())
            {
                Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio pagamentoMotoristaIntegracaoEnvio = new Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio();
                pagamentoMotoristaIntegracaoEnvio.Data = DateTime.Now.Date;
                pagamentoMotoristaIntegracaoEnvio.NumeroTentativas = 0;
                pagamentoMotoristaIntegracaoEnvio.PagamentoMotoristaTMS = pagamentoMotorista;
                pagamentoMotoristaIntegracaoEnvio.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                pagamentoMotoristaIntegracaoEnvio.TipoIntegracaoPagamentoMotorista = tipoIntegracaoPagamentoMotorista;

                repPagamentoMotoristaIntegracaoEnvio.Inserir(pagamentoMotoristaIntegracaoEnvio);

                if (configuracaoIntegracaoKMM?.PossuiIntegracao ?? false)
                    Servicos.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS.AdicionarIntegracaoKMM(pagamentoMotorista, unidadeDeTrabalho);
            }
            else if (pagamentoMotorista.SituacaoPagamentoMotorista == SituacaoPagamentoMotorista.AgIntegracao)
            {
                if (configuracaoIntegracaoKMM?.PossuiIntegracao ?? false)
                {
                    Servicos.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS.AdicionarIntegracaoKMM(pagamentoMotorista, unidadeDeTrabalho);
                }
                else
                {
                    pagamentoMotorista.SituacaoPagamentoMotorista = SituacaoPagamentoMotorista.Finalizada;
                    pagamentoMotorista.EtapaPagamentoMotorista = EtapaPagamentoMotorista.Integracao;

                    if (configuracaoTMS.ConfirmarPagamentoMotoristaAutomaticamente)
                    {
                        msgRetorno = "";
                        Servicos.Embarcador.PagamentoMotorista.AutorizacaoPagamentoMotorista.ConfirmarPagamentoMotorista(ref msgRetorno, pagamentoMotorista.Codigo, configuracaoTMS.TipoMovimentoPagamentoMotorista, auditado, pagamentoMotorista.Usuario, unidadeDeTrabalho, unidadeDeTrabalho.StringConexao, tipoServicoMultisoftware);
                    }
                }
            }

            if (configuraoAcertoViagem?.HabilitarFormaRecebimentoTituloAoMotorista ?? false)
                saldoViagemMotorista = motorista.SaldoAdiantamento * -1;

            if (!this.GerarControleSaldoMotorista(ref msgRetorno, acertoViagem, unidadeDeTrabalho, usuario, auditado, tipoServicoMultisoftware, stringConexao, configuracaoTMS.ExibirSaldoPrevistoAcertoViagem, configuracaoTMS.VisualizarReciboPorMotoristaNoAcertoDeViagem, saldoViagemMotorista))
                return false;

            acertoViagem.ValorMovimentadoFichaMotorista = saldoViagemMotorista;
            repAcertoViagem.Atualizar(acertoViagem);

            return true;
        }


        public bool GerarControleSaldoMotorista(ref string msgRetorno, Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem, Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.Entidades.Usuario usuario, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string stringConexao, bool exibirSaldoPrevistoAcertoViagem, bool visualizarReciboPorMotoristaNoAcertoDeViagem, decimal saldoViagemMotorista)
        {
            if (auditado == null)
            {
                auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
                {
                    TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema,
                    OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema
                };
            }
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unidadeDeTrabalho);
            Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unidadeDeTrabalho);

            Dominio.Entidades.Usuario motorista = repUsuario.BuscarPorCodigo(acertoViagem.Motorista.Codigo, true);

            msgRetorno = "";
            if (visualizarReciboPorMotoristaNoAcertoDeViagem)
            {
                motorista.SaldoAdiantamento += saldoViagemMotorista;
                Servicos.Embarcador.PagamentoMotorista.AutorizacaoPagamentoMotorista.CriarHistoricoMovimentacaoSaldoMotorista(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoMotorista.Adiantamento, saldoViagemMotorista, motorista, usuario, unidadeDeTrabalho, null, acertoViagem, acertoViagem.DataFechamento.Value, auditado, tipoServicoMultisoftware);
                repUsuario.Atualizar(motorista, auditado);

                acertoViagem.ValorMovimentadoFichaMotorista = saldoViagemMotorista;
                repAcertoViagem.Atualizar(acertoViagem);
            }
            else
            {
                if (acertoViagem.ValorAdiantamentoSaldo > 0)
                {
                    motorista.SaldoAdiantamento += acertoViagem.ValorAdiantamentoSaldo;
                    Servicos.Embarcador.PagamentoMotorista.AutorizacaoPagamentoMotorista.CriarHistoricoMovimentacaoSaldoMotorista(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoMotorista.Adiantamento, acertoViagem.ValorAdiantamentoSaldo, motorista, usuario, unidadeDeTrabalho, null, acertoViagem, acertoViagem.DataFechamento.Value, auditado, tipoServicoMultisoftware);
                    repUsuario.Atualizar(motorista, auditado);
                }
                else if (exibirSaldoPrevistoAcertoViagem && acertoViagem.ValorAdiantamentoSaldo != 0)
                {
                    motorista.SaldoAdiantamento += acertoViagem.ValorAdiantamentoSaldo;
                    Servicos.Embarcador.PagamentoMotorista.AutorizacaoPagamentoMotorista.CriarHistoricoMovimentacaoSaldoMotorista(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoMotorista.Adiantamento, acertoViagem.ValorAdiantamentoSaldo, motorista, usuario, unidadeDeTrabalho, null, acertoViagem, acertoViagem.DataFechamento.Value, auditado, tipoServicoMultisoftware);
                    repUsuario.Atualizar(motorista, auditado);
                }
                else if (acertoViagem.ValorAdiantamentoSaldo < 0)
                {
                    if (!Servicos.Embarcador.PagamentoMotorista.AutorizacaoPagamentoMotorista.CriarPagamentoMotoristaSaldo(ref msgRetorno, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoMotorista.Adiantamento, (acertoViagem.ValorAdiantamentoSaldo * -1), motorista, usuario, unidadeDeTrabalho, null, acertoViagem, acertoViagem.DataFechamento.Value, auditado, tipoServicoMultisoftware, stringConexao))
                        return false;
                }

                if (acertoViagem.ValorAlimentacaoSaldo > 0)
                {
                    motorista.SaldoDiaria += acertoViagem.ValorAlimentacaoSaldo;
                    Servicos.Embarcador.PagamentoMotorista.AutorizacaoPagamentoMotorista.CriarHistoricoMovimentacaoSaldoMotorista(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoMotorista.Diaria, acertoViagem.ValorAlimentacaoSaldo, motorista, usuario, unidadeDeTrabalho, null, acertoViagem, acertoViagem.DataFechamento.Value, auditado, tipoServicoMultisoftware);
                    repUsuario.Atualizar(motorista, auditado);
                }
                else if (exibirSaldoPrevistoAcertoViagem && acertoViagem.ValorAlimentacaoSaldo != 0)
                {
                    motorista.SaldoDiaria += acertoViagem.ValorAlimentacaoSaldo;
                    Servicos.Embarcador.PagamentoMotorista.AutorizacaoPagamentoMotorista.CriarHistoricoMovimentacaoSaldoMotorista(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoMotorista.Diaria, acertoViagem.ValorAlimentacaoSaldo, motorista, usuario, unidadeDeTrabalho, null, acertoViagem, acertoViagem.DataFechamento.Value, auditado, tipoServicoMultisoftware);
                    repUsuario.Atualizar(motorista, auditado);
                }
                else if (acertoViagem.ValorAlimentacaoSaldo < 0)
                {
                    if (!Servicos.Embarcador.PagamentoMotorista.AutorizacaoPagamentoMotorista.CriarPagamentoMotoristaSaldo(ref msgRetorno, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoMotorista.Diaria, (acertoViagem.ValorAlimentacaoSaldo * -1), motorista, usuario, unidadeDeTrabalho, null, acertoViagem, acertoViagem.DataFechamento.Value, auditado, tipoServicoMultisoftware, stringConexao))
                        return false;
                }
            }

            return true;
        }

        public bool EstornarControleSaldoMotorista(ref string msgRetorno, Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem, Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.Entidades.Usuario usuario, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string stringConexao, bool exibirSaldoPrevistoAcertoViagem, bool visualizarReciboPorMotoristaNoAcertoDeViagem, decimal saldoViagemMotorista)
        {
            if (auditado == null)
            {
                auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
                {
                    TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema,
                    OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema
                };
            }

            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unidadeDeTrabalho);
            Dominio.Entidades.Usuario motorista = repUsuario.BuscarPorCodigo(acertoViagem.Motorista.Codigo, true);

            msgRetorno = "";
            if (visualizarReciboPorMotoristaNoAcertoDeViagem)
            {
                motorista.SaldoAdiantamento -= saldoViagemMotorista;
                Servicos.Embarcador.PagamentoMotorista.AutorizacaoPagamentoMotorista.CriarHistoricoMovimentacaoSaldoMotorista(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoMotorista.Adiantamento, (saldoViagemMotorista * -1), motorista, usuario, unidadeDeTrabalho, null, acertoViagem, acertoViagem.DataFechamento.Value, auditado, tipoServicoMultisoftware);
                repUsuario.Atualizar(motorista, auditado);
            }
            else
            {
                if (acertoViagem.ValorAdiantamentoSaldo > 0)
                {
                    motorista.SaldoAdiantamento -= acertoViagem.ValorAdiantamentoSaldo;
                    Servicos.Embarcador.PagamentoMotorista.AutorizacaoPagamentoMotorista.CriarHistoricoMovimentacaoSaldoMotorista(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoMotorista.Adiantamento, (acertoViagem.ValorAdiantamentoSaldo * -1), motorista, usuario, unidadeDeTrabalho, null, acertoViagem, acertoViagem.DataFechamento.Value, auditado, tipoServicoMultisoftware);
                    repUsuario.Atualizar(motorista, auditado);
                }
                else if (exibirSaldoPrevistoAcertoViagem && acertoViagem.ValorAdiantamentoSaldo != 0)
                {
                    motorista.SaldoAdiantamento += (acertoViagem.ValorAdiantamentoSaldo * -1);
                    Servicos.Embarcador.PagamentoMotorista.AutorizacaoPagamentoMotorista.CriarHistoricoMovimentacaoSaldoMotorista(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoMotorista.Adiantamento, acertoViagem.ValorAdiantamentoSaldo, motorista, usuario, unidadeDeTrabalho, null, acertoViagem, acertoViagem.DataFechamento.Value, auditado, tipoServicoMultisoftware);
                    repUsuario.Atualizar(motorista, auditado);
                }

                if (acertoViagem.ValorAlimentacaoSaldo > 0)
                {
                    motorista.SaldoDiaria -= acertoViagem.ValorAlimentacaoSaldo;
                    Servicos.Embarcador.PagamentoMotorista.AutorizacaoPagamentoMotorista.CriarHistoricoMovimentacaoSaldoMotorista(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoMotorista.Diaria, (acertoViagem.ValorAlimentacaoSaldo * -1), motorista, usuario, unidadeDeTrabalho, null, acertoViagem, acertoViagem.DataFechamento.Value, auditado, tipoServicoMultisoftware);
                    repUsuario.Atualizar(motorista, auditado);
                }
                else if (exibirSaldoPrevistoAcertoViagem && acertoViagem.ValorAlimentacaoSaldo != 0)
                {
                    motorista.SaldoDiaria += (acertoViagem.ValorAlimentacaoSaldo * -1);
                    Servicos.Embarcador.PagamentoMotorista.AutorizacaoPagamentoMotorista.CriarHistoricoMovimentacaoSaldoMotorista(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoMotorista.Diaria, acertoViagem.ValorAlimentacaoSaldo, motorista, usuario, unidadeDeTrabalho, null, acertoViagem, acertoViagem.DataFechamento.Value, auditado, tipoServicoMultisoftware);
                    repUsuario.Atualizar(motorista, auditado);
                }
            }

            return true;
        }

        public decimal RetornarPreviaValorDiaria(Dominio.Entidades.Embarcador.Acerto.AcertoViagem acerto, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unitOfWork);
            Repositorio.Embarcador.Acerto.AcertoDiaria repAcertoDiaria = new Repositorio.Embarcador.Acerto.AcertoDiaria(unitOfWork);
            Repositorio.Embarcador.Acerto.TabelaDiaria repTabelaDiaria = new Repositorio.Embarcador.Acerto.TabelaDiaria(unitOfWork);
            Repositorio.Embarcador.Acerto.TabelaDiariaPeriodo repTabelaDiariaPeriodo = new Repositorio.Embarcador.Acerto.TabelaDiariaPeriodo(unitOfWork);

            if (acerto == null)
                return 0;

            DateTime dataInicial = acerto.DataInicial;
            DateTime dataFinal = DateTime.Now;
            if (acerto.DataFinal.HasValue && acerto.DataFinal.Value > DateTime.MinValue)
                dataFinal = acerto.DataFinal.Value;

            if (dataInicial == null || dataInicial == DateTime.MinValue)
                return 0;

            List<Dominio.Entidades.Embarcador.Acerto.TabelaDiaria> tabelaDiaria = null;
            Dominio.Entidades.Embarcador.Acerto.TabelaDiariaPeriodo periodoDaVigencia = null;
            List<int> codigosTabelas = new List<int>();

            tabelaDiaria = repTabelaDiaria.BuscarTabelaDiaria(acerto.SegmentoVeiculo?.Codigo ?? 0, acerto.Cargas.Where(o => o.Carga.ModeloVeicularCarga != null).Select(o => o.Carga.ModeloVeicularCarga.Codigo).ToList(), acerto.Motorista.CentroResultado?.Codigo ?? 0);
            if ((tabelaDiaria == null || tabelaDiaria.Count == 0) && acerto.Motorista.CentroResultado != null)
                tabelaDiaria = repTabelaDiaria.BuscarTabelaDiariaPorCentroResultado(acerto.Motorista.CentroResultado?.Codigo ?? 0);
            if ((tabelaDiaria == null || tabelaDiaria.Count == 0))
                tabelaDiaria = repTabelaDiaria.BuscarTabelaDiaria(acerto.SegmentoVeiculo?.Codigo ?? 0);
            if ((tabelaDiaria == null || tabelaDiaria.Count == 0))
                tabelaDiaria = repTabelaDiaria.BuscarTabelaDiaria(acerto.Cargas.Where(o => o.Carga.ModeloVeicularCarga != null).Select(o => o.Carga.ModeloVeicularCarga.Codigo).ToList());
            if ((tabelaDiaria == null || tabelaDiaria.Count == 0))
                tabelaDiaria = repTabelaDiaria.BuscarTabelaDiaria();

            if (tabelaDiaria == null || tabelaDiaria.Count == 0)
                return 0;

            codigosTabelas = tabelaDiaria.Select(c => c.Codigo).Distinct().ToList();

            List<Dominio.Entidades.Embarcador.Acerto.TabelaDiariaPeriodo> periodos = repTabelaDiariaPeriodo.BuscarPorTabela(codigosTabelas.FirstOrDefault());
            if (periodos == null || periodos.Count <= 0)
                return 0;

            decimal valorPreviaDiaria = 0;
            DateTime dataFinalDia;
            bool encontrouRefeicao = false;
            while (dataInicial <= dataFinal)
            {
                dataFinalDia = new DateTime(dataInicial.Year, dataInicial.Month, dataInicial.Day, 23, 59, 59);
                if (dataFinalDia > dataFinal)
                    dataFinalDia = dataFinal;

                foreach (var periodo in periodos)
                {
                    if (periodo.HoraInicial.Value.Hours == 0 && periodo.HoraFinal.HasValue)
                    {
                        if (dataInicial.TimeOfDay <= periodo.HoraFinal.Value)
                        {
                            periodoDaVigencia = repTabelaDiariaPeriodo.BuscarPorVigencia(codigosTabelas, periodo.Descricao, dataInicial.Date);
                            if (periodoDaVigencia == null)
                                periodoDaVigencia = periodo;
                            valorPreviaDiaria += periodoDaVigencia.Valor;
                            encontrouRefeicao = true;
                        }
                    }
                    else if (periodo.HoraInicial.Value.Hours > 0 && periodo.HoraFinal.Value.Hours < 23)
                    {
                        if ((dataInicial.TimeOfDay >= periodo.HoraInicial.Value || dataInicial.TimeOfDay <= periodo.HoraInicial.Value) && (dataFinalDia.TimeOfDay >= periodo.HoraFinal.Value || dataFinalDia.TimeOfDay <= periodo.HoraFinal.Value)
                            && (dataFinalDia.TimeOfDay >= periodo.HoraInicial.Value && periodo.HoraFinal.Value >= dataInicial.TimeOfDay))
                        {
                            periodoDaVigencia = repTabelaDiariaPeriodo.BuscarPorVigencia(codigosTabelas, periodo.Descricao, dataInicial.Date);
                            if (periodoDaVigencia == null)
                                periodoDaVigencia = periodo;
                            valorPreviaDiaria += periodoDaVigencia.Valor;
                            encontrouRefeicao = true;
                        }
                    }
                    else if (periodo.HoraInicial.HasValue && periodo.HoraFinal.Value.Hours == 23)
                    {
                        if (dataFinalDia.TimeOfDay >= periodo.HoraInicial.Value)
                        {
                            periodoDaVigencia = repTabelaDiariaPeriodo.BuscarPorVigencia(codigosTabelas, periodo.Descricao, dataInicial.Date);
                            if (periodoDaVigencia == null)
                                periodoDaVigencia = periodo;
                            valorPreviaDiaria += periodoDaVigencia.Valor;
                            encontrouRefeicao = true;
                        }
                    }
                }

                dataInicial = dataInicial.AddDays(1);
                dataInicial = new DateTime(dataInicial.Year, dataInicial.Month, dataInicial.Day, 5, 00, 00);
            }
            if (!encontrouRefeicao)
            {
                dataInicial = acerto.DataInicial;
                Dominio.Entidades.Embarcador.Acerto.TabelaDiariaPeriodo periodo = null;
                periodo = periodos.Where(o => o.HoraInicial <= dataInicial.TimeOfDay && (!o.HoraFinal.HasValue || o.HoraFinal.Value.Hours == 0)).FirstOrDefault();//para buscar a janta
                if (periodo == null)
                    periodo = periodos.Where(o => o.HoraInicial <= dataInicial.TimeOfDay && o.HoraFinal.Value.Hours > 0 && o.HoraInicial.Value.Hours > 0).FirstOrDefault();//para buscar o almoço
                if (periodo == null)
                    periodo = periodos.Where(o => o.HoraFinal <= dataInicial.TimeOfDay && (!o.HoraInicial.HasValue || o.HoraInicial.Value.Hours == 0)).FirstOrDefault();//para buscar o café
                if (periodo != null)
                {
                    periodoDaVigencia = repTabelaDiariaPeriodo.BuscarPorVigencia(codigosTabelas, periodo.Descricao, dataInicial.Date);
                    if (periodoDaVigencia == null)
                        periodoDaVigencia = periodo;
                    valorPreviaDiaria += periodoDaVigencia.Valor;
                }
            }

            return valorPreviaDiaria;
        }

        #endregion
    }
}
