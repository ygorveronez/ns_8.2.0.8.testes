using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Integracao.Diaria
{
    public class DiariaMotorista
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public DiariaMotorista(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public void RecalcularPagamentoMotoristaEmbarcador(DateTime novaDataFinal, Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> pedidos, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS repositorioPagamentoMotoristaTMS = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS(_unitOfWork);
            Repositorio.Embarcador.PagamentoMotorista.DespesaViagem repositorioDespesaViagem = new Repositorio.Embarcador.PagamentoMotorista.DespesaViagem(_unitOfWork);
            Repositorio.Embarcador.Acerto.TabelaDiaria repositorioTabelaDiaria = new Repositorio.Embarcador.Acerto.TabelaDiaria(_unitOfWork);
            Repositorio.Embarcador.Acerto.TabelaDiariaPeriodo repositorioTabelaDiariaPediodo = new Repositorio.Embarcador.Acerto.TabelaDiariaPeriodo(_unitOfWork);

            Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamentoMotoristaTMS = repositorioPagamentoMotoristaTMS.BuscarFirstOrDefaultPorCarga(carga.Codigo);

            if (pagamentoMotoristaTMS == null)
                return;

            DateTime maiorDataDosPedidos = carga.DataPrevisaoTerminoCarga ?? carga.DataFimViagemPrevista ?? DateTime.MinValue;

            if (novaDataFinal <= maiorDataDosPedidos)
                return;

            if (pagamentoMotoristaTMS.PagamentoLiberado)
                return;

            Dominio.Entidades.Usuario motorista = carga.Motoristas.FirstOrDefault();

            if (motorista == null)
                return;

            List<Dominio.Entidades.Embarcador.PagamentoMotorista.DespesaViagem> listaDespesasExistentes = repositorioDespesaViagem.BuscarEntidadesPorPagamentoMotorista(pagamentoMotoristaTMS.Codigo);

            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = pedidos?.FirstOrDefault();

            DateTime dataInicial = cargaPedido.Pedido.DataPrevisaoSaida.HasValue ? cargaPedido.Pedido.DataPrevisaoSaida.Value : carga.DataInicialPrevisaoCarregamento.HasValue ? carga.DataInicialPrevisaoCarregamento.Value : DateTime.MinValue;
            DateTime dataFinal = novaDataFinal;

            List<int> codigosModelos = new List<int>();
            if (carga.ModeloVeicularCarga != null)
                codigosModelos.Add(carga.ModeloVeicularCarga.Codigo);

            List<int> codigosTabelas = new List<int>();
            List<Dominio.Entidades.Embarcador.Acerto.TabelaDiaria> tabelaDiaria = repositorioTabelaDiaria.BuscarPorModeloVeicularFilial(codigosModelos, (carga.Filial?.Codigo ?? 0));

            if ((tabelaDiaria == null || tabelaDiaria.Count == 0) && motorista.CentroResultado != null)
                tabelaDiaria = repositorioTabelaDiaria.BuscarTabelaDiariaPorCentroResultado(motorista.CentroResultado.Codigo);
            if ((tabelaDiaria == null || tabelaDiaria.Count == 0))
                tabelaDiaria = repositorioTabelaDiaria.BuscarTabelaDiaria(carga.Veiculo?.SegmentoVeiculo?.Codigo ?? 0);
            if ((tabelaDiaria == null || tabelaDiaria.Count == 0))
                tabelaDiaria = repositorioTabelaDiaria.BuscarTabelaDiaria(codigosModelos);
            if ((tabelaDiaria == null || tabelaDiaria.Count == 0))
                tabelaDiaria = repositorioTabelaDiaria.BuscarTabelaDiaria();

            if (tabelaDiaria == null || tabelaDiaria.Count == 0)
                return;

            codigosTabelas = tabelaDiaria.Select(c => c.Codigo).Distinct().ToList();

            List<Dominio.Entidades.Embarcador.Acerto.TabelaDiariaPeriodo> periodos = repositorioTabelaDiariaPediodo.BuscarPorTabela(codigosTabelas?.FirstOrDefault() ?? 0);

            if (periodos.Count == 0)
                return;

            foreach (Dominio.Entidades.Embarcador.PagamentoMotorista.DespesaViagem despesaExistente in listaDespesasExistentes)
                repositorioDespesaViagem.Deletar(despesaExistente);

            pagamentoMotoristaTMS.Observacao = "Diária gerada a partir da carga nº " + carga.CodigoCargaEmbarcador + ". Viagem inicia em " + dataInicial.ToString("dd/MM/yyyy HH:mm") + " até " + novaDataFinal.ToString("dd/MM/yyyy HH:mm");
            pagamentoMotoristaTMS.Valor = 0;

            DateTime dataFinalDia;
            bool encontrouRefeicao = false;
            bool teveDespesa = false;

            List<Dominio.Entidades.Embarcador.PagamentoMotorista.DespesaViagem> despesas = new List<Dominio.Entidades.Embarcador.PagamentoMotorista.DespesaViagem>();
            Dominio.Entidades.Embarcador.Acerto.TabelaDiariaPeriodo periodoDaVigencia = null;

            while (dataInicial <= dataFinal)
            {
                dataFinalDia = new DateTime(dataInicial.Year, dataInicial.Month, dataInicial.Day, 23, 59, 59);
                if (dataFinalDia > dataFinal)
                    dataFinalDia = dataFinal;

                foreach (var periodo in periodos)
                {
                    teveDespesa = false;
                    if (periodo.HoraInicial.Value.Hours == 0 && periodo.HoraFinal.HasValue)
                    {
                        if (dataInicial.TimeOfDay <= periodo.HoraFinal.Value)
                        {
                            periodoDaVigencia = repositorioTabelaDiariaPediodo.BuscarPorVigencia(codigosTabelas, periodo.Descricao, dataInicial.Date);
                            if (periodoDaVigencia == null)
                                periodoDaVigencia = periodo;
                            InserirPeriodoDiaria(ref pagamentoMotoristaTMS, periodoDaVigencia.Valor, " - " + periodoDaVigencia.Descricao + " " + dataInicial.ToString("dd/MM/yyyy") + " Valor: " + periodoDaVigencia.Valor.ToString("n2"));
                            encontrouRefeicao = true;
                            teveDespesa = true;
                        }
                    }
                    else if (periodo.HoraInicial.Value.Hours > 0 && periodo.HoraFinal.Value.Hours < 23)
                    {
                        if ((dataInicial.TimeOfDay >= periodo.HoraInicial.Value || dataInicial.TimeOfDay <= periodo.HoraInicial.Value) && (dataFinalDia.TimeOfDay >= periodo.HoraFinal.Value || dataFinalDia.TimeOfDay <= periodo.HoraFinal.Value)
                            && (dataFinalDia.TimeOfDay >= periodo.HoraInicial.Value) && (dataInicial.TimeOfDay <= periodo.HoraInicial.Value))
                        {
                            periodoDaVigencia = repositorioTabelaDiariaPediodo.BuscarPorVigencia(codigosTabelas, periodo.Descricao, dataInicial.Date);
                            if (periodoDaVigencia == null)
                                periodoDaVigencia = periodo;
                            InserirPeriodoDiaria(ref pagamentoMotoristaTMS, periodoDaVigencia.Valor, " - " + periodoDaVigencia.Descricao + " " + dataInicial.ToString("dd/MM/yyyy") + " Valor: " + periodoDaVigencia.Valor.ToString("n2"));
                            encontrouRefeicao = true;
                            teveDespesa = true;
                        }
                    }
                    else if (periodo.HoraInicial.HasValue && periodo.HoraFinal.Value.Hours == 23)
                    {
                        if (dataFinalDia.TimeOfDay >= periodo.HoraInicial.Value)
                        {
                            periodoDaVigencia = repositorioTabelaDiariaPediodo.BuscarPorVigencia(codigosTabelas, periodo.Descricao, dataInicial.Date);
                            if (periodoDaVigencia == null)
                                periodoDaVigencia = periodo;
                            InserirPeriodoDiaria(ref pagamentoMotoristaTMS, periodoDaVigencia.Valor, " - " + periodoDaVigencia.Descricao + " " + dataInicial.ToString("dd/MM/yyyy") + " Valor: " + periodoDaVigencia.Valor.ToString("n2"));
                            encontrouRefeicao = true;
                            teveDespesa = true;
                        }
                    }

                    if (teveDespesa)
                        AdicionarRegistroDespesa(pagamentoMotoristaTMS, periodo, despesas);
                }

                dataInicial = dataInicial.AddDays(1);
                dataInicial = new DateTime(dataInicial.Year, dataInicial.Month, dataInicial.Day, 5, 00, 00);
            }

            decimal saldoDescontado = 0;
            if (pagamentoMotoristaTMS.PagamentoMotoristaTipo.TipoPagamentoMotorista != TipoPagamentoMotorista.Nenhum && pagamentoMotoristaTMS.PagamentoMotoristaTipo.TipoPagamentoMotorista != TipoPagamentoMotorista.Terceiro)
                saldoDescontado = pagamentoMotoristaTMS.PagamentoMotoristaTipo.TipoPagamentoMotorista == TipoPagamentoMotorista.Diaria ? pagamentoMotoristaTMS.Motorista.SaldoDiaria : pagamentoMotoristaTMS.Motorista.SaldoAdiantamento;
            if (saldoDescontado > pagamentoMotoristaTMS.Valor)
                saldoDescontado = pagamentoMotoristaTMS.Valor;
            pagamentoMotoristaTMS.SaldoDescontado = saldoDescontado;
            pagamentoMotoristaTMS.SaldoDiariaMotorista = pagamentoMotoristaTMS.Motorista?.SaldoDiaria ?? 0;

            repositorioPagamentoMotoristaTMS.Atualizar(pagamentoMotoristaTMS, auditado);

            foreach (Dominio.Entidades.Embarcador.PagamentoMotorista.DespesaViagem despesa in despesas)
                repositorioDespesaViagem.Inserir(despesa);
        }

        public static bool GerarPagamentoMotoristaTerceiro(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS repPagamentoMotorista = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS(unitOfWork);
                Repositorio.Embarcador.PagamentoMotorista.DespesaViagem repDespesaViagem = new Repositorio.Embarcador.PagamentoMotorista.DespesaViagem(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio repPagamentoMotoristaIntegracaoEnvio = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio(unitOfWork);
                Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas repModalidadeTransportadoraPessoas = new Repositorio.Embarcador.Pessoas.ModalidadeTransportadoraPessoas(unitOfWork);
                Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);

                if (carga == null || carga.Motoristas == null || carga.Motoristas.Count == 0)
                    return true;

                Dominio.Entidades.Usuario motorista = carga.Motoristas.FirstOrDefault();
                Dominio.Entidades.Cliente terceiro = carga.Terceiro;

                if (terceiro == null)
                    return true;

                Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTransportadoraPessoas = repModalidadeTransportadoraPessoas.BuscarPorPessoa(terceiro.CPF_CNPJ);
                Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete = repContratoFrete.BuscarPorCarga(carga.Codigo);

                if (motorista == null || terceiro == null || modalidadeTransportadoraPessoas == null || modalidadeTransportadoraPessoas.PagamentoMotoristaTipo == null || contratoFrete == null || !modalidadeTransportadoraPessoas.GerarPagamentoTerceiro)
                    return true;

                if (repPagamentoMotorista.PagamentoGerado(carga.Codigo))
                    return true;

                Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamentoMotoristaExiste = repPagamentoMotorista.BuscarFirstOrDefaultPorCarga(carga.Codigo);

                Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
                {
                    TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema,
                    Usuario = carga?.Operador,
                    Empresa = carga?.Operador?.Empresa,
                    Texto = ""
                };

                if (pagamentoMotoristaExiste != null && !pagamentoMotoristaExiste.PagamentoLiberado)
                {
                    Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio pagamentoEnvioExiste = repPagamentoMotoristaIntegracaoEnvio.BuscarPorPagamento(pagamentoMotoristaExiste.Codigo);

                    if (pagamentoEnvioExiste != null) repPagamentoMotoristaIntegracaoEnvio.Deletar(pagamentoEnvioExiste);

                    List<Dominio.Entidades.Embarcador.PagamentoMotorista.DespesaViagem> listaDespesasExistentes = repDespesaViagem.BuscarEntidadesPorPagamentoMotorista(pagamentoMotoristaExiste.Codigo);

                    foreach (Dominio.Entidades.Embarcador.PagamentoMotorista.DespesaViagem despesaExistente in listaDespesasExistentes)
                        repDespesaViagem.Deletar(despesaExistente);

                    repPagamentoMotorista.Deletar(pagamentoMotoristaExiste);
                }

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamentoMotorista = new Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS()
                {
                    Carga = carga,
                    Data = DateTime.Now.Date,
                    DataPagamento = DateTime.Now,
                    EtapaPagamentoMotorista = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaPagamentoMotorista.Iniciada,
                    Motorista = motorista,
                    Terceiro = terceiro,
                    Numero = repPagamentoMotorista.BuscarProximoNumero(),
                    PagamentoMotoristaTipo = modalidadeTransportadoraPessoas.PagamentoMotoristaTipo,
                    PlanoDeContaCredito = null,
                    PlanoDeContaDebito = null,
                    SituacaoPagamentoMotorista = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.AgInformacoes,
                    Observacao = "Referente ao contrato da carga " + carga.CodigoCargaEmbarcador + ". Valor: " + (contratoFrete.ValorFreteSubcontratacao - contratoFrete.ValorImpostosReter).ToString("n2") + " Adiantamento: " + contratoFrete.ValorAdiantamento.ToString("n2") + " Saldo: " + contratoFrete.ValorSaldo.ToString("n2"),
                    Usuario = carga.Operador,
                    PagamentoLiberado = true,
                    Valor = contratoFrete.ValorFreteSubcontratacao - contratoFrete.ValorImpostosReter,
                    ValorAdiantamento = contratoFrete.ValorAdiantamento,
                    DataAdiantamento = DateTime.Now.Date.AddDays(contratoFrete.DiasVencimentoAdiantamento),
                    DataSaldo = Servicos.Embarcador.Carga.FreteSubcontratacaoTerceiro.ObterVencimentoSaldoContrato(contratoFrete)
                };

                if (pagamentoMotorista.Observacao.Length > 5000)
                    pagamentoMotorista.Observacao = pagamentoMotorista.Observacao.Substring(0, 4999);

                decimal saldoDescontado = 0;
                pagamentoMotorista.SaldoDescontado = saldoDescontado;
                pagamentoMotorista.SaldoDiariaMotorista = pagamentoMotorista.Motorista?.SaldoDiaria ?? 0;

                Servicos.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS.CalcularImpostos(ref pagamentoMotorista, unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);

                repPagamentoMotorista.Inserir(pagamentoMotorista);

                if (VerificarRegrasAutorizacaoPagamentoMotorista(pagamentoMotorista, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador, unitOfWork, carga.Operador, auditado))
                {
                    pagamentoMotorista.SituacaoPagamentoMotorista = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.AutorizacaoPendente;
                    pagamentoMotorista.EtapaPagamentoMotorista = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaPagamentoMotorista.AgAutorizacao;
                }
                else
                {
                    pagamentoMotorista.SituacaoPagamentoMotorista = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.AgIntegracao;
                    pagamentoMotorista.EtapaPagamentoMotorista = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaPagamentoMotorista.Integracao;
                }

                Repositorio.Embarcador.Configuracoes.IntegracaoKMM repositorioIntegracaoKMM = new Repositorio.Embarcador.Configuracoes.IntegracaoKMM(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKMM configuracaoIntegracaoKMM = repositorioIntegracaoKMM.BuscarPrimeiroRegistro();

                TipoIntegracaoPagamentoMotorista tipoIntegracaoPagamentoMotorista = pagamentoMotorista.PagamentoMotoristaTipo.TipoIntegracaoPagamentoMotorista;
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
                        Servicos.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS.AdicionarIntegracaoKMM(pagamentoMotorista, unitOfWork);
                }
                else if (pagamentoMotorista.SituacaoPagamentoMotorista == SituacaoPagamentoMotorista.AgIntegracao)
                {
                    if (configuracaoIntegracaoKMM?.PossuiIntegracao ?? false)
                    {
                        Servicos.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS.AdicionarIntegracaoKMM(pagamentoMotorista, unitOfWork);
                    }
                    else
                    {
                        pagamentoMotorista.SituacaoPagamentoMotorista = SituacaoPagamentoMotorista.Finalizada;
                        pagamentoMotorista.EtapaPagamentoMotorista = EtapaPagamentoMotorista.Integracao;

                        if (configuracaoTMS.ConfirmarPagamentoMotoristaAutomaticamente)
                        {
                            string msgRetorno = "";
                            Servicos.Embarcador.PagamentoMotorista.AutorizacaoPagamentoMotorista.ConfirmarPagamentoMotorista(ref msgRetorno, pagamentoMotorista.Codigo, configuracaoTMS.TipoMovimentoPagamentoMotorista, null, carga.Operador, unitOfWork, unitOfWork.StringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return true;
            }
        }

        public void GerarPagamentoMotoristaEmbarcador(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            if (carga.Empresa?.PagamentoMotoristaTipo == null)
                return;

            if (!(carga.TipoOperacao?.GerarDiariaMotoristaProprio ?? false))
                return;

            Repositorio.Embarcador.Acerto.TabelaDiaria repTabelaDiaria = new Repositorio.Embarcador.Acerto.TabelaDiaria(_unitOfWork);
            Repositorio.Embarcador.Acerto.TabelaDiariaPeriodo repTabelaDiariaPeriodo = new Repositorio.Embarcador.Acerto.TabelaDiariaPeriodo(_unitOfWork);
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS repPagamentoMotorista = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS(_unitOfWork);
            Repositorio.Embarcador.PagamentoMotorista.DespesaViagem repDespesaViagem = new Repositorio.Embarcador.PagamentoMotorista.DespesaViagem(_unitOfWork);
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio repPagamentoMotoristaIntegracaoEnvio = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio(_unitOfWork);

            Dominio.Entidades.Usuario motorista = carga.Motoristas != null && carga.Motoristas.Count > 0 ? carga.Motoristas.FirstOrDefault() : null;
            Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamentoMotoristaExiste = repPagamentoMotorista.BuscarFirstOrDefaultPorCarga(carga.Codigo);

            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
            {
                TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema,
                Usuario = carga?.Operador,
                Empresa = carga?.Operador?.Empresa,
                Texto = ""
            };

            if (pagamentoMotoristaExiste != null && !pagamentoMotoristaExiste.PagamentoLiberado)
            {
                Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio pagamentoEnvioExiste = repPagamentoMotoristaIntegracaoEnvio.BuscarPorPagamento(pagamentoMotoristaExiste.Codigo);

                if (pagamentoEnvioExiste != null) repPagamentoMotoristaIntegracaoEnvio.Deletar(pagamentoEnvioExiste);

                List<Dominio.Entidades.Embarcador.PagamentoMotorista.DespesaViagem> listaDespesasExistentes = repDespesaViagem.BuscarEntidadesPorPagamentoMotorista(pagamentoMotoristaExiste.Codigo);

                foreach (Dominio.Entidades.Embarcador.PagamentoMotorista.DespesaViagem despesaExistente in listaDespesasExistentes)
                    repDespesaViagem.Deletar(despesaExistente);

                repPagamentoMotorista.Deletar(pagamentoMotoristaExiste, auditado);
            }

            if (motorista == null)
                return;

            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = carga.Pedidos?.FirstOrDefault();

            DateTime? dataPrevisaoRetorno = carga.DataRetornoCD ?? ObterDataPrevisaoRetorno(carga);
            DateTime dataInicial = cargaPedido.Pedido.DataPrevisaoSaida.HasValue ? cargaPedido.Pedido.DataPrevisaoSaida.Value : carga.DataInicialPrevisaoCarregamento.HasValue ? carga.DataInicialPrevisaoCarregamento.Value : carga.DataCriacaoCarga;
            DateTime dataFinal = carga.DataRetornoCD ?? dataPrevisaoRetorno ?? carga.Pedidos?.Max(x => x.Pedido?.PrevisaoEntrega) ?? carga.DataFimViagemPrevista ?? DateTime.Today;

            if (dataInicial > dataFinal)
                return;

            Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamentoMotorista = new Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS()
            {
                Carga = carga,
                Data = DateTime.Now.Date,
                DataPagamento = dataInicial,
                EtapaPagamentoMotorista = EtapaPagamentoMotorista.Iniciada,
                Motorista = motorista,
                Numero = repPagamentoMotorista.BuscarProximoNumero(),
                PagamentoMotoristaTipo = carga.Empresa.PagamentoMotoristaTipo,
                PlanoDeContaCredito = null,
                PlanoDeContaDebito = null,
                SituacaoPagamentoMotorista = SituacaoPagamentoMotorista.AgInformacoes,
                Observacao = "Diária gerada a partir da carga nº " + carga.CodigoCargaEmbarcador + ". Viagem inicia em " + dataInicial.ToString("dd/MM/yyyy HH:mm") + " até " + dataFinal.ToString("dd/MM/yyyy HH:mm"),
                Usuario = carga.Operador,
                Valor = 0,
                PagamentoLiberado = false
            };

            List<int> codigosModelos = new List<int>();
            if (carga.ModeloVeicularCarga != null)
                codigosModelos.Add(carga.ModeloVeicularCarga.Codigo);

            List<int> codigosTabelas = new List<int>();
            List<Dominio.Entidades.Embarcador.Acerto.TabelaDiaria> tabelaDiaria = repTabelaDiaria.BuscarPorModeloVeicularFilial(codigosModelos, (carga.Filial?.Codigo ?? 0));

            if ((tabelaDiaria == null || tabelaDiaria.Count == 0) && motorista.CentroResultado != null)
                tabelaDiaria = repTabelaDiaria.BuscarTabelaDiariaPorCentroResultado(motorista.CentroResultado.Codigo);
            if ((tabelaDiaria == null || tabelaDiaria.Count == 0))
                tabelaDiaria = repTabelaDiaria.BuscarTabelaDiaria(carga.Veiculo?.SegmentoVeiculo?.Codigo ?? 0);
            if ((tabelaDiaria == null || tabelaDiaria.Count == 0))
                tabelaDiaria = repTabelaDiaria.BuscarTabelaDiaria(codigosModelos);
            if ((tabelaDiaria == null || tabelaDiaria.Count == 0))
                tabelaDiaria = repTabelaDiaria.BuscarTabelaDiaria();

            if (tabelaDiaria == null || tabelaDiaria.Count == 0)
                return;

            codigosTabelas = tabelaDiaria.Select(c => c.Codigo).Distinct().ToList();

            List<Dominio.Entidades.Embarcador.Acerto.TabelaDiariaPeriodo> periodos = repTabelaDiariaPeriodo.BuscarPorTabela(codigosTabelas?.FirstOrDefault() ?? 0);

            if (periodos.Count == 0)
                return;

            DateTime dataFinalDia;
            bool encontrouRefeicao = false;
            bool teveDespesa = false;

            List<Dominio.Entidades.Embarcador.PagamentoMotorista.DespesaViagem> despesas = new List<Dominio.Entidades.Embarcador.PagamentoMotorista.DespesaViagem>();
            Dominio.Entidades.Embarcador.Acerto.TabelaDiariaPeriodo periodoDaVigencia = null;
            while (dataInicial <= dataFinal)
            {
                dataFinalDia = new DateTime(dataInicial.Year, dataInicial.Month, dataInicial.Day, 23, 59, 59);
                if (dataFinalDia > dataFinal)
                    dataFinalDia = dataFinal;

                foreach (var periodo in periodos)
                {
                    teveDespesa = false;
                    if (periodo.HoraInicial.Value.Hours == 0 && periodo.HoraFinal.HasValue)
                    {
                        if (dataInicial.TimeOfDay <= periodo.HoraFinal.Value)
                        {
                            periodoDaVigencia = repTabelaDiariaPeriodo.BuscarPorVigencia(codigosTabelas, periodo.Descricao, dataInicial.Date);
                            if (periodoDaVigencia == null)
                                periodoDaVigencia = periodo;
                            InserirPeriodoDiaria(ref pagamentoMotorista, periodoDaVigencia.Valor, " - " + periodoDaVigencia.Descricao + " " + dataInicial.ToString("dd/MM/yyyy") + " Valor: " + periodoDaVigencia.Valor.ToString("n2"));
                            encontrouRefeicao = true;
                            teveDespesa = true;
                        }
                    }
                    else if (periodo.HoraInicial.Value.Hours > 0 && periodo.HoraFinal.Value.Hours < 23)
                    {
                        if ((dataInicial.TimeOfDay >= periodo.HoraInicial.Value || dataInicial.TimeOfDay <= periodo.HoraInicial.Value) && (dataFinalDia.TimeOfDay >= periodo.HoraFinal.Value || dataFinalDia.TimeOfDay <= periodo.HoraFinal.Value)
                            && (dataFinalDia.TimeOfDay >= periodo.HoraInicial.Value) && (dataInicial.TimeOfDay <= periodo.HoraInicial.Value))
                        {
                            periodoDaVigencia = repTabelaDiariaPeriodo.BuscarPorVigencia(codigosTabelas, periodo.Descricao, dataInicial.Date);
                            if (periodoDaVigencia == null)
                                periodoDaVigencia = periodo;
                            InserirPeriodoDiaria(ref pagamentoMotorista, periodoDaVigencia.Valor, " - " + periodoDaVigencia.Descricao + " " + dataInicial.ToString("dd/MM/yyyy") + " Valor: " + periodoDaVigencia.Valor.ToString("n2"));
                            encontrouRefeicao = true;
                            teveDespesa = true;
                        }
                    }
                    else if (periodo.HoraInicial.HasValue && periodo.HoraFinal.Value.Hours == 23)
                    {
                        if (dataFinalDia.TimeOfDay >= periodo.HoraInicial.Value)
                        {
                            periodoDaVigencia = repTabelaDiariaPeriodo.BuscarPorVigencia(codigosTabelas, periodo.Descricao, dataInicial.Date);
                            if (periodoDaVigencia == null)
                                periodoDaVigencia = periodo;
                            InserirPeriodoDiaria(ref pagamentoMotorista, periodoDaVigencia.Valor, " - " + periodoDaVigencia.Descricao + " " + dataInicial.ToString("dd/MM/yyyy") + " Valor: " + periodoDaVigencia.Valor.ToString("n2"));
                            encontrouRefeicao = true;
                            teveDespesa = true;
                        }
                    }

                    if (teveDespesa)
                        AdicionarRegistroDespesa(pagamentoMotorista, periodo, despesas);
                }

                dataInicial = dataInicial.AddDays(1);
                dataInicial = new DateTime(dataInicial.Year, dataInicial.Month, dataInicial.Day, 5, 00, 00);
            }

            if (pagamentoMotorista.Observacao.Length > 5000)
                pagamentoMotorista.Observacao = pagamentoMotorista.Observacao.Substring(0, 4999);

            decimal saldoDescontado = 0;
            if (pagamentoMotorista.PagamentoMotoristaTipo.TipoPagamentoMotorista != TipoPagamentoMotorista.Nenhum && pagamentoMotorista.PagamentoMotoristaTipo.TipoPagamentoMotorista != TipoPagamentoMotorista.Terceiro)
                saldoDescontado = pagamentoMotorista.PagamentoMotoristaTipo.TipoPagamentoMotorista == TipoPagamentoMotorista.Diaria ? pagamentoMotorista.Motorista.SaldoDiaria : pagamentoMotorista.Motorista.SaldoAdiantamento;
            if (saldoDescontado > pagamentoMotorista.Valor)
                saldoDescontado = pagamentoMotorista.Valor;
            pagamentoMotorista.SaldoDescontado = saldoDescontado;
            pagamentoMotorista.SaldoDiariaMotorista = pagamentoMotorista.Motorista?.SaldoDiaria ?? 0;

            Servicos.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS.CalcularImpostos(ref pagamentoMotorista, _unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);

            repPagamentoMotorista.Inserir(pagamentoMotorista, auditado);

            foreach (Dominio.Entidades.Embarcador.PagamentoMotorista.DespesaViagem despesa in despesas)
                repDespesaViagem.Inserir(despesa);

            if (VerificarRegrasAutorizacaoPagamentoMotorista(pagamentoMotorista, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador, _unitOfWork, carga.Operador, auditado))
            {
                pagamentoMotorista.SituacaoPagamentoMotorista = SituacaoPagamentoMotorista.AutorizacaoPendente;
                pagamentoMotorista.EtapaPagamentoMotorista = EtapaPagamentoMotorista.AgAutorizacao;
            }
            else
            {
                pagamentoMotorista.SituacaoPagamentoMotorista = SituacaoPagamentoMotorista.AgIntegracao;
                pagamentoMotorista.EtapaPagamentoMotorista = EtapaPagamentoMotorista.Integracao;
            }

            Repositorio.Embarcador.Configuracoes.IntegracaoKMM repositorioIntegracaoKMM = new Repositorio.Embarcador.Configuracoes.IntegracaoKMM(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKMM configuracaoIntegracaoKMM = repositorioIntegracaoKMM.BuscarPrimeiroRegistro();

            TipoIntegracaoPagamentoMotorista tipoIntegracaoPagamentoMotorista = pagamentoMotorista.PagamentoMotoristaTipo.TipoIntegracaoPagamentoMotorista;
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
                    Servicos.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS.AdicionarIntegracaoKMM(pagamentoMotorista, _unitOfWork);
            }
            else if (pagamentoMotorista.SituacaoPagamentoMotorista == SituacaoPagamentoMotorista.AgIntegracao)
            {
                if (configuracaoIntegracaoKMM?.PossuiIntegracao ?? false)
                {
                    Servicos.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS.AdicionarIntegracaoKMM(pagamentoMotorista, _unitOfWork);
                }
                else
                {
                    pagamentoMotorista.SituacaoPagamentoMotorista = SituacaoPagamentoMotorista.Finalizada;
                    pagamentoMotorista.EtapaPagamentoMotorista = EtapaPagamentoMotorista.Integracao;

                    if (configuracao.ConfirmarPagamentoMotoristaAutomaticamente)
                    {
                        string msgRetorno = "";
                        Servicos.Embarcador.PagamentoMotorista.AutorizacaoPagamentoMotorista.ConfirmarPagamentoMotorista(ref msgRetorno, pagamentoMotorista.Codigo, configuracao.TipoMovimentoPagamentoMotorista, null, carga.Operador, _unitOfWork, _unitOfWork.StringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);
                    }
                }
            }
        }

        public static void IntegrarCarga(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Acerto.TabelaDiaria repTabelaDiaria = new Repositorio.Embarcador.Acerto.TabelaDiaria(unitOfWork);
            Repositorio.Embarcador.Acerto.TabelaDiariaPeriodo repTabelaDiariaPeriodo = new Repositorio.Embarcador.Acerto.TabelaDiariaPeriodo(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork);
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS repPagamentoMotorista = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS(unitOfWork);
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio repPagamentoMotoristaIntegracaoEnvio = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
            {
                TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema,
                Usuario = cargaIntegracao.Carga?.Operador,
                Empresa = cargaIntegracao.Carga?.Operador?.Empresa,
                Texto = ""
            };

            cargaIntegracao.NumeroTentativas += 1;

            if (cargaIntegracao.Carga == null || cargaIntegracao.Carga.TipoOperacao == null || cargaIntegracao.Carga.TipoOperacao.GerarDiariaMotoristaProprio != true || cargaIntegracao.Carga.TipoOperacao.PagamentoMotoristaTipo == null || cargaIntegracao.Carga.Rota == null || cargaIntegracao.Carga.Rota.TempoDeViagemEmMinutos <= 0)
            {
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = "Carga, Tipo de Operação e/ou Rota não estão configurados para gerar a diária automáticamente.";
                repCargaIntegracao.Atualizar(cargaIntegracao);
                return;
            }
            Dominio.Entidades.Usuario motorista = cargaIntegracao.Carga.Motoristas.FirstOrDefault();
            if (motorista == null || motorista.TipoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotorista.Terceiro)
            {
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = "Motorista próprio não localizado nesta carga.";
                repCargaIntegracao.Atualizar(cargaIntegracao);
                return;
            }

            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = cargaIntegracao.Carga.Pedidos.FirstOrDefault();
            List<int> codigosTabelas = new List<int>();

            List<Dominio.Entidades.Embarcador.Acerto.TabelaDiaria> tabelaDiaria = null;
            List<int> codigosModelos = new List<int>();
            if (cargaIntegracao.Carga.ModeloVeicularCarga != null)
                codigosModelos.Add(cargaIntegracao.Carga.ModeloVeicularCarga.Codigo);

            tabelaDiaria = repTabelaDiaria.BuscarTabelaDiaria(cargaIntegracao.Carga.Veiculo?.SegmentoVeiculo?.Codigo ?? 0, codigosModelos, motorista.CentroResultado?.Codigo ?? 0);

            if ((tabelaDiaria == null || tabelaDiaria.Count == 0) && motorista.CentroResultado != null)
                tabelaDiaria = repTabelaDiaria.BuscarTabelaDiariaPorCentroResultado(motorista.CentroResultado.Codigo);
            if ((tabelaDiaria == null || tabelaDiaria.Count == 0))
                tabelaDiaria = repTabelaDiaria.BuscarTabelaDiaria(cargaIntegracao.Carga.Veiculo?.SegmentoVeiculo?.Codigo ?? 0);
            if ((tabelaDiaria == null || tabelaDiaria.Count == 0))
                tabelaDiaria = repTabelaDiaria.BuscarTabelaDiaria(codigosModelos);
            if ((tabelaDiaria == null || tabelaDiaria.Count == 0))
                tabelaDiaria = repTabelaDiaria.BuscarTabelaDiaria();

            if (tabelaDiaria == null || tabelaDiaria.Count == 0)
            {
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = "Tabela de diária não localizada conforme carga selecionada.";
                repCargaIntegracao.Atualizar(cargaIntegracao);
                return;
            }

            codigosTabelas = tabelaDiaria.Select(c => c.Codigo).Distinct().ToList();

            List<Dominio.Entidades.Embarcador.Acerto.TabelaDiariaPeriodo> periodos = repTabelaDiariaPeriodo.BuscarPorTabela(codigosTabelas.FirstOrDefault());
            if (periodos == null || periodos.Count <= 0)
            {
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = "Tabela de diária sem períodos cadastrados.";
                repCargaIntegracao.Atualizar(cargaIntegracao);
                return;
            }

            DateTime dataInicial = cargaPedido.Pedido.DataPrevisaoSaida.HasValue ? cargaPedido.Pedido.DataPrevisaoSaida.Value : cargaIntegracao.Carga.DataInicialPrevisaoCarregamento.HasValue ? cargaIntegracao.Carga.DataInicialPrevisaoCarregamento.Value : DateTime.MinValue;

            if (dataInicial == DateTime.MinValue)
            {
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = "Carga sem data de previsão de inicio.";
                repCargaIntegracao.Atualizar(cargaIntegracao);
                return;
            }

            double tempoDeViagem = (int)Math.Ceiling((decimal)cargaIntegracao.Carga.Rota.TempoDeViagemEmMinutos / 60);
            tempoDeViagem += 1;//Adicionado uma hora por solicitação padrão da Transben
            tempoDeViagem = Math.Round(tempoDeViagem * (1.45 + ((tempoDeViagem / 12) / 15)));//Cálculo passado pela Transben chamado de número 490

            if (tempoDeViagem <= 0)
            {
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = "O cálculo do tempo de viagem está incorreto.";
                repCargaIntegracao.Atualizar(cargaIntegracao);
                return;
            }

            DateTime dataFinal = dataInicial.AddHours(tempoDeViagem);

            if (dataFinal == DateTime.MinValue || dataFinal == dataInicial)
            {
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = "Carga sem data de previsão de fim.";
                repCargaIntegracao.Atualizar(cargaIntegracao);
                return;
            }

            Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamentoMotorista = new Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS()
            {
                Carga = cargaIntegracao.Carga,
                Data = DateTime.Now.Date,
                DataPagamento = dataInicial,
                EtapaPagamentoMotorista = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaPagamentoMotorista.Iniciada,
                Motorista = motorista,
                Numero = repPagamentoMotorista.BuscarProximoNumero(),
                PagamentoMotoristaTipo = cargaIntegracao.Carga.TipoOperacao.PagamentoMotoristaTipo,
                PlanoDeContaCredito = null,
                PlanoDeContaDebito = null,
                SituacaoPagamentoMotorista = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.AgInformacoes,
                Observacao = "Diária gerada a partir da carga nº " + cargaIntegracao.Carga.CodigoCargaEmbarcador + ". Viagem inicia em " + dataInicial.ToString("dd/MM/yyyy HH:mm") + " até " + dataFinal.ToString("dd/MM/yyyy HH:mm"),
                Usuario = cargaIntegracao.Carga.Operador,
                Valor = 0,
                PagamentoLiberado = true
            };

            DateTime dataFinalDia;
            bool encontrouRefeicao = false;
            Dominio.Entidades.Embarcador.Acerto.TabelaDiariaPeriodo periodoDaVigencia = null;

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
                            InserirPeriodoDiaria(ref pagamentoMotorista, periodoDaVigencia.Valor, " - " + periodoDaVigencia.Descricao + " " + dataInicial.ToString("dd/MM/yyyy") + " Valor: " + periodoDaVigencia.Valor.ToString("n2"));
                            encontrouRefeicao = true;
                        }
                    }
                    else if (periodo.HoraInicial.Value.Hours > 0 && periodo.HoraFinal.Value.Hours < 23)
                    {
                        if ((dataInicial.TimeOfDay >= periodo.HoraInicial.Value || dataInicial.TimeOfDay <= periodo.HoraInicial.Value) && (dataFinalDia.TimeOfDay >= periodo.HoraFinal.Value || dataFinalDia.TimeOfDay <= periodo.HoraFinal.Value)
                            && (dataFinalDia.TimeOfDay >= periodo.HoraInicial.Value) && (dataInicial.TimeOfDay <= periodo.HoraInicial.Value))
                        {
                            periodoDaVigencia = repTabelaDiariaPeriodo.BuscarPorVigencia(codigosTabelas, periodo.Descricao, dataInicial.Date);
                            if (periodoDaVigencia == null)
                                periodoDaVigencia = periodo;
                            InserirPeriodoDiaria(ref pagamentoMotorista, periodoDaVigencia.Valor, " - " + periodoDaVigencia.Descricao + " " + dataInicial.ToString("dd/MM/yyyy") + " Valor: " + periodoDaVigencia.Valor.ToString("n2"));
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
                            InserirPeriodoDiaria(ref pagamentoMotorista, periodoDaVigencia.Valor, " - " + periodoDaVigencia.Descricao + " " + dataInicial.ToString("dd/MM/yyyy") + " Valor: " + periodoDaVigencia.Valor.ToString("n2"));
                            encontrouRefeicao = true;
                        }
                    }

                }

                dataInicial = dataInicial.AddDays(1);
                dataInicial = new DateTime(dataInicial.Year, dataInicial.Month, dataInicial.Day, 5, 00, 00);
            }
            //para quando precisa pagar a refeição do período anterior
            if (!encontrouRefeicao)
            {
                dataInicial = cargaPedido.Pedido.DataPrevisaoSaida.HasValue ? cargaPedido.Pedido.DataPrevisaoSaida.Value : cargaIntegracao.Carga.DataInicialPrevisaoCarregamento.HasValue ? cargaIntegracao.Carga.DataInicialPrevisaoCarregamento.Value : DateTime.MinValue;
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
                    InserirPeriodoDiaria(ref pagamentoMotorista, periodoDaVigencia.Valor, " - " + periodoDaVigencia.Descricao + " " + dataInicial.ToString("dd/MM/yyyy") + " Valor: " + periodoDaVigencia.Valor.ToString("n2"));
                }
            }

            if (pagamentoMotorista.Observacao.Length > 5000)
                pagamentoMotorista.Observacao = pagamentoMotorista.Observacao.Substring(0, 4999);

            decimal saldoDescontado = 0;
            if (pagamentoMotorista.PagamentoMotoristaTipo.TipoPagamentoMotorista != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoMotorista.Nenhum && pagamentoMotorista.PagamentoMotoristaTipo.TipoPagamentoMotorista != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoMotorista.Terceiro)
                saldoDescontado = pagamentoMotorista.PagamentoMotoristaTipo.TipoPagamentoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoMotorista.Diaria ? pagamentoMotorista.Motorista.SaldoDiaria : pagamentoMotorista.Motorista.SaldoAdiantamento;
            if (saldoDescontado > pagamentoMotorista.Valor)
                saldoDescontado = pagamentoMotorista.Valor;
            pagamentoMotorista.SaldoDescontado = saldoDescontado;
            pagamentoMotorista.SaldoDiariaMotorista = pagamentoMotorista.Motorista?.SaldoDiaria ?? 0;
            pagamentoMotorista.PagamentoLiberado = true;

            Servicos.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS.CalcularImpostos(ref pagamentoMotorista, unitOfWork, tipoServicoMultisoftware);

            repPagamentoMotorista.Inserir(pagamentoMotorista);

            if (VerificarRegrasAutorizacaoPagamentoMotorista(pagamentoMotorista, tipoServicoMultisoftware, unitOfWork, cargaIntegracao.Carga.Operador, auditado))
            {
                pagamentoMotorista.SituacaoPagamentoMotorista = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.AutorizacaoPendente;
                pagamentoMotorista.EtapaPagamentoMotorista = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaPagamentoMotorista.AgAutorizacao;
            }
            else
            {
                pagamentoMotorista.SituacaoPagamentoMotorista = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.AgIntegracao;
                pagamentoMotorista.EtapaPagamentoMotorista = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaPagamentoMotorista.Integracao;
            }

            Repositorio.Embarcador.Configuracoes.IntegracaoKMM repositorioIntegracaoKMM = new Repositorio.Embarcador.Configuracoes.IntegracaoKMM(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKMM configuracaoIntegracaoKMM = repositorioIntegracaoKMM.BuscarPrimeiroRegistro();

            TipoIntegracaoPagamentoMotorista tipoIntegracaoPagamentoMotorista = pagamentoMotorista.PagamentoMotoristaTipo.TipoIntegracaoPagamentoMotorista;
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
                    Servicos.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS.AdicionarIntegracaoKMM(pagamentoMotorista, unitOfWork);
            }
            else if (pagamentoMotorista.SituacaoPagamentoMotorista == SituacaoPagamentoMotorista.AgIntegracao)
            {
                if (configuracaoIntegracaoKMM?.PossuiIntegracao ?? false)
                {
                    Servicos.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS.AdicionarIntegracaoKMM(pagamentoMotorista, unitOfWork);
                }
                else
                {
                    pagamentoMotorista.SituacaoPagamentoMotorista = SituacaoPagamentoMotorista.Finalizada;
                    pagamentoMotorista.EtapaPagamentoMotorista = EtapaPagamentoMotorista.Integracao;

                    if (configuracaoTMS.ConfirmarPagamentoMotoristaAutomaticamente)
                    {
                        string msgRetorno = "";
                        Servicos.Embarcador.PagamentoMotorista.AutorizacaoPagamentoMotorista.ConfirmarPagamentoMotorista(ref msgRetorno, pagamentoMotorista.Codigo, configuracaoTMS.TipoMovimentoPagamentoMotorista, null, cargaIntegracao.Carga.Operador, unitOfWork, unitOfWork.StringConexao, tipoServicoMultisoftware);
                    }
                }
            }



            cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
            cargaIntegracao.ProblemaIntegracao = "Diária(s) gerada(s) com sucesso.";
            cargaIntegracao.Protocolo = "";
            repCargaIntegracao.Atualizar(cargaIntegracao);
        }

        public DateTime? ObterDataPrevisaoRetorno(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem repositorioRotaFretePontosPassagem = new Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem cargaRotaFreteRetorno = repositorioRotaFretePontosPassagem.BuscarPontoRetornoPorCarga(carga.Codigo);

            if (cargaRotaFreteRetorno == null)
                return null;

            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(_unitOfWork);
            DateTime? dataUltimaEntrega = repositorioCargaEntrega.BuscarPrevisaoUltimaEntregaPorCarga(carga.Codigo);

            if (!dataUltimaEntrega.HasValue)
                return null;

            DateTime dataPrevisaoRetorno = dataUltimaEntrega.Value
                .AddHours(2)
                .AddMinutes(cargaRotaFreteRetorno.Tempo);

            return dataPrevisaoRetorno;
        }

        #endregion

        #region Métodos Privados

        private static void InserirPeriodoDiaria(ref Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamentoMotorista, decimal valor, string observacao)
        {
            pagamentoMotorista.Valor += valor;
            pagamentoMotorista.Observacao += observacao;
        }

        private static void AdicionarRegistroDespesa(Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamentoMotorista, Dominio.Entidades.Embarcador.Acerto.TabelaDiariaPeriodo periodo, List<Dominio.Entidades.Embarcador.PagamentoMotorista.DespesaViagem> despesas)
        {
            Dominio.Entidades.Embarcador.PagamentoMotorista.DespesaViagem despesa = new Dominio.Entidades.Embarcador.PagamentoMotorista.DespesaViagem()
            {
                PagamentoMotoristaTMS = pagamentoMotorista,
                TabelaDiariaPeriodo = periodo
            };

            despesas.Add(despesa);
        }

        private static bool VerificarRegrasAutorizacaoPagamentoMotorista(Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamentoMotorista, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Usuario usuario, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            List<Dominio.Entidades.Embarcador.PagamentoMotorista.RegrasPagamentoMotorista> listaFiltrada = Servicos.Embarcador.PagamentoMotorista.AutorizacaoPagamentoMotorista.VerificarRegrasPagamentoMotorista(pagamentoMotorista, unitOfWork);

            if (listaFiltrada.Count() > 0)
            {
                Servicos.Embarcador.PagamentoMotorista.AutorizacaoPagamentoMotorista.CriarRegrasAutorizacao(listaFiltrada, pagamentoMotorista, usuario, tipoServicoMultisoftware, unitOfWork.StringConexao, unitOfWork, auditado, out bool contemAprovadorIgualAoOperador);
                return true;
            }

            return false;
        }

        #endregion
    }
}
