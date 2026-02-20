using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Fatura;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Fatura
{
    public class FaturamentoLote : ServicoBase
    {
        public FaturamentoLote() : base() { }

        public static void ProcessarFaturamentoLote(Repositorio.UnitOfWork unidadeTrabalho, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Fatura.FaturamentoLote repFaturamentoLote = new Repositorio.Embarcador.Fatura.FaturamentoLote(unidadeTrabalho);

            List<long> codigos = repFaturamentoLote.BuscarCodigosPorSituacao(TipoFaturamentoLote.Faturamento, SituacaoEnvioDocumentacaoLote.Aguardando, "Codigo", "asc", 0, 100);
            for (var i = 0; i < codigos.Count; i++)
            {
                Dominio.Entidades.Embarcador.Fatura.FaturamentoLote faturamentoLote = repFaturamentoLote.BuscarPorCodigo(codigos[i]);

                DateTime dataAFaturar = DateTime.Now;
                if (!string.IsNullOrWhiteSpace(faturamentoLote.Empresa?.FusoHorario ?? ""))
                {
                    TimeZoneInfo fusoHorarioEmpresa = TimeZoneInfo.FindSystemTimeZoneById(faturamentoLote.Empresa?.FusoHorario);
                    dataAFaturar = TimeZoneInfo.ConvertTime(dataAFaturar, TimeZoneInfo.Local, fusoHorarioEmpresa);
                }
                if (!faturamentoLote.DataAFaturar.HasValue || faturamentoLote.DataAFaturar.Value == DateTime.MinValue || faturamentoLote.DataAFaturar.Value <= dataAFaturar)//pegar hora do fuso
                    GerarFaturamentoLote(out string erro, faturamentoLote, unidadeTrabalho, stringConexao, tipoServicoMultisoftware);
            }

            codigos = repFaturamentoLote.BuscarCodigosPorSituacao(TipoFaturamentoLote.Cancelamento, SituacaoEnvioDocumentacaoLote.Aguardando, "Codigo", "asc", 0, 3);
            for (var i = 0; i < codigos.Count; i++)
            {
                Dominio.Entidades.Embarcador.Fatura.FaturamentoLote faturamentoLote = repFaturamentoLote.BuscarPorCodigo(codigos[i]);

                GerarCancelamentoLote(out string erro, faturamentoLote, unidadeTrabalho, stringConexao, tipoServicoMultisoftware);
            }

            Servicos.Embarcador.Fatura.FaturamentoLote.GerarFaturamentoAutomatico(unidadeTrabalho, stringConexao, tipoServicoMultisoftware);
            Servicos.Embarcador.Fatura.FaturamentoLote.ProcessarNotificacaoFaturamentoLote(unidadeTrabalho, stringConexao, tipoServicoMultisoftware);
            Servicos.Embarcador.Fatura.FaturamentoLote.ProcessarNotificacaoCancelamentoFaturamentoLote(unidadeTrabalho, stringConexao, tipoServicoMultisoftware);
            Servicos.Embarcador.Fatura.FaturamentoLote.ProcessarNotificacaoCancelamentoFaturamentoManual(unidadeTrabalho, stringConexao, tipoServicoMultisoftware);
        }

        public static void GerarFaturamentoAutomatico(Repositorio.UnitOfWork unidadeTrabalho, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            try
            {
                Repositorio.Embarcador.Fatura.FaturamentoLote repFaturamentoLote = new Repositorio.Embarcador.Fatura.FaturamentoLote(unidadeTrabalho);
                Repositorio.Embarcador.Pedidos.PedidoViagemNavio repPedidoViagemNavio = new Repositorio.Embarcador.Pedidos.PedidoViagemNavio(unidadeTrabalho);
                Repositorio.Embarcador.Pedidos.PedidoViagemNavioSchedule repSchedule = new Repositorio.Embarcador.Pedidos.PedidoViagemNavioSchedule(unidadeTrabalho);
                Repositorio.Embarcador.Pedidos.TipoTerminalImportacao repTerminal = new Repositorio.Embarcador.Pedidos.TipoTerminalImportacao(unidadeTrabalho);
                Repositorio.Embarcador.Pedidos.Porto repPorto = new Repositorio.Embarcador.Pedidos.Porto(unidadeTrabalho);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(unidadeTrabalho);

                DateTime dataHoraAtual = DateTime.Now;
                DateTime dataAFaturar = DateTime.Now;
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();

                IList<Dominio.ObjetosDeValor.Embarcador.Fatura.FaturamentoAutomatico> faturamentosPendentes = repFaturamentoLote.BuscarFaturamentoAutomaticoPendente(dataHoraAtual, configuracaoFinanceiro.DelayFaturamentoAutomatico);
                for (var i = 0; i < faturamentosPendentes.Count; i++)
                {
                    unidadeTrabalho.Start();

                    dataAFaturar = RetornarDataFaturar(faturamentosPendentes[i].DataSaidaNavio, faturamentosPendentes[i].QuantidadeHoras, unidadeTrabalho);
                    if (!string.IsNullOrWhiteSpace(faturamentosPendentes[i].Fuso))
                    {
                        TimeZoneInfo fusoHorarioEmpresa = TimeZoneInfo.FindSystemTimeZoneById(faturamentosPendentes[i].Fuso);
                        dataAFaturar = TimeZoneInfo.ConvertTime(dataAFaturar, TimeZoneInfo.Local, fusoHorarioEmpresa);
                    }

                    if (dataAFaturar.Hour < 8 || dataAFaturar.Hour >= 17)
                    {
                        if (dataAFaturar.Hour >= 17)
                        {
                            var dataLimiteFatura = new DateTime(dataAFaturar.Year, dataAFaturar.Month, dataAFaturar.Day, 17, 00, 00);
                            TimeSpan tsDiferenca = dataAFaturar - dataLimiteFatura;
                            if (tsDiferenca != null && (tsDiferenca.TotalHours > 0 || tsDiferenca.TotalMinutes > 0))
                            {
                                dataAFaturar = dataAFaturar.AddDays(1);
                                dataAFaturar = Utilidades.Conversor.ChangeTime(dataAFaturar, 8, 0, 0, 0);
                                dataAFaturar = dataAFaturar.AddHours(tsDiferenca.TotalHours);
                                dataAFaturar = RetornarProximaDataValida(dataAFaturar, unidadeTrabalho);
                            }
                        }
                        else if (dataAFaturar.Hour < 8)
                        {
                            DateTime dataLimiteFatura;
                            if (dataAFaturar.Day > 1)
                                dataLimiteFatura = new DateTime(dataAFaturar.Year, dataAFaturar.Month, dataAFaturar.Day > 1 ? dataAFaturar.Day - 1 : dataAFaturar.Day, 17, 00, 00);
                            else
                            {
                                dataLimiteFatura = new DateTime(dataAFaturar.Year, dataAFaturar.Month > 1 ? dataAFaturar.Month - 1 : dataAFaturar.Month, dataAFaturar.Day, 17, 00, 00);
                                dataLimiteFatura = new DateTime(dataLimiteFatura.Year, dataLimiteFatura.Month, dataLimiteFatura.LastDayOfMonth().Day, 17, 00, 00);
                            }

                            TimeSpan tsDiferenca = dataAFaturar - dataLimiteFatura;
                            if (tsDiferenca != null && (tsDiferenca.TotalHours > 0 || tsDiferenca.TotalMinutes > 0))
                            {
                                //dataAFaturar = dataAFaturar.AddDays(1);
                                dataAFaturar = Utilidades.Conversor.ChangeTime(dataAFaturar, 8, 0, 0, 0);
                                dataAFaturar = dataAFaturar.AddHours(tsDiferenca.TotalHours);
                                dataAFaturar = RetornarProximaDataValida(dataAFaturar, unidadeTrabalho);
                                if (dataAFaturar.Hour >= 17)
                                {
                                    dataLimiteFatura = new DateTime(dataAFaturar.Year, dataAFaturar.Month, dataAFaturar.Day, 17, 00, 00);
                                    tsDiferenca = dataAFaturar - dataLimiteFatura;
                                    if (tsDiferenca != null && (tsDiferenca.TotalHours > 0 || tsDiferenca.TotalMinutes > 0))
                                    {
                                        dataAFaturar = dataAFaturar.AddDays(1);
                                        dataAFaturar = Utilidades.Conversor.ChangeTime(dataAFaturar, 8, 0, 0, 0);
                                        dataAFaturar = dataAFaturar.AddHours(tsDiferenca.TotalHours);
                                        dataAFaturar = RetornarProximaDataValida(dataAFaturar, unidadeTrabalho);
                                    }
                                }
                            }
                        }

                    }

                    Dominio.Entidades.Embarcador.Fatura.FaturamentoLote faturamentoLote = new Dominio.Entidades.Embarcador.Fatura.FaturamentoLote()
                    {
                        Cliente = null,
                        DataFatura = DateTime.Now.Date,
                        DataFinal = null,
                        DataGeracao = DateTime.Now,
                        DataInicial = null,
                        Destino = null,
                        GrupoPessoas = null,
                        NumeroBooking = string.Empty,
                        Observacao = string.Empty,
                        Origem = null,
                        PedidoViagemNavio = faturamentosPendentes[i].Viagem > 0 ? repPedidoViagemNavio.BuscarPorCodigo(faturamentosPendentes[i].Viagem) : null,
                        Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEnvioDocumentacaoLote.Aguardando,
                        TerminalDestino = null,
                        TerminalOrigem = faturamentosPendentes[i].Terminal > 0 ? repTerminal.BuscarPorCodigo(faturamentosPendentes[i].Terminal) : null,
                        Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFaturamentoLote.Faturamento,
                        TipoOperacao = null,
                        TipoPessoa = TipoPessoa.GrupoPessoa,
                        Usuario = null,
                        Empresa = faturamentosPendentes[i].Porto > 0 ? repPorto.BuscarPorCodigo(faturamentosPendentes[i].Porto)?.Empresa : null,
                        FaturamentoAutomatico = true,
                        NotificadoOperador = false,
                        DataAFaturar = dataAFaturar,
                        TipoCTe = Dominio.Enumeradores.TipoCTE.Normal
                    };

                    if (faturamentoLote.TipoPropostaMultimodal == null)
                        faturamentoLote.TipoPropostaMultimodal = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal>();

                    faturamentoLote.TipoPropostaMultimodal.Add(TipoPropostaMultimodal.CargaFechada);
                    faturamentoLote.TipoPropostaMultimodal.Add(TipoPropostaMultimodal.CargaFracionada);
                    faturamentoLote.TipoPropostaMultimodal.Add(TipoPropostaMultimodal.Feeder);

                    repFaturamentoLote.Inserir(faturamentoLote);

                    Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavioSchedule schedule = repSchedule.BuscarPorCodigo(faturamentosPendentes[i].Codigo);
                    schedule.GerouFaturamentoAutomatico = true;
                    repSchedule.Atualizar(schedule);

                    unidadeTrabalho.CommitChanges();

                    unidadeTrabalho.FlushAndClear();
                }
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
            }
        }

        public static void ProcessarNotificacaoFaturamentoLote(Repositorio.UnitOfWork unidadeTrabalho, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            try
            {
                Repositorio.Embarcador.Fatura.FaturamentoLote repFaturamentoLote = new Repositorio.Embarcador.Fatura.FaturamentoLote(unidadeTrabalho);
                Repositorio.Embarcador.Pedidos.PedidoViagemNavio repPedidoViagemNavio = new Repositorio.Embarcador.Pedidos.PedidoViagemNavio(unidadeTrabalho);
                Repositorio.Embarcador.Pedidos.PedidoViagemNavioSchedule repSchedule = new Repositorio.Embarcador.Pedidos.PedidoViagemNavioSchedule(unidadeTrabalho);
                Repositorio.Embarcador.Pedidos.TipoTerminalImportacao repTerminal = new Repositorio.Embarcador.Pedidos.TipoTerminalImportacao(unidadeTrabalho);
                Repositorio.Embarcador.Pedidos.Porto repPorto = new Repositorio.Embarcador.Pedidos.Porto(unidadeTrabalho);

                IList<Dominio.ObjetosDeValor.Embarcador.Fatura.FaturamentoLoteNotificacao> faturamentosAgrupadosPendentes = repFaturamentoLote.BuscarFaturamentoAutomaticoPendenteNotificacao(true);
                if (faturamentosAgrupadosPendentes != null && faturamentosAgrupadosPendentes.Count > 0)
                {
                    List<long> codigosFaturamentoLote = faturamentosAgrupadosPendentes.Select(c => c.Codigo).Distinct().ToList();
                    for (var i = 0; i < codigosFaturamentoLote.Count; i++)
                    {
                        List<int> faturas = faturamentosAgrupadosPendentes.Where(c => c.Codigo == codigosFaturamentoLote[i]).Select(c => c.Fatura).Distinct().ToList();
                        var lista = faturamentosAgrupadosPendentes.Where(c => c.Codigo == codigosFaturamentoLote[i]).ToList();
                        List<double> cnpjsPessoas = lista.Where(p => (double)p.Cliente > 0).Select(p => (double)p.Cliente).Distinct().ToList();
                        List<int> codigosGrupoPessoas = lista.Where(p => (int)p.Grupo > 0).Select(p => (int)p.Grupo).Distinct().ToList();
                        if (cnpjsPessoas != null && cnpjsPessoas.Count > 0)
                        {
                            foreach (var cnpjPessoa in cnpjsPessoas)
                            {
                                if (cnpjPessoa > 0)
                                {
                                    List<int> codigosFatura = lista.Where(p => (double)p.Cliente == cnpjPessoa).Select(p => (int)p.Fatura).Distinct().ToList();
                                    Servicos.Embarcador.Fatura.Fatura.EnviarFaturaLote(codigosFatura, stringConexao, tipoServicoMultisoftware, null, null);
                                }
                            }
                        }
                        if (codigosGrupoPessoas != null && codigosGrupoPessoas.Count > 0)
                        {
                            foreach (var codigoGrupoPessoa in codigosGrupoPessoas)
                            {
                                if (codigoGrupoPessoa > 0)
                                {
                                    List<int> codigosFatura = lista.Where(p => (int)p.Grupo == codigoGrupoPessoa).Select(p => (int)p.Fatura).Distinct().ToList();
                                    Servicos.Embarcador.Fatura.Fatura.EnviarFaturaLote(codigosFatura, stringConexao, tipoServicoMultisoftware, null, null);
                                }
                            }
                        }

                    }
                }
                IList<Dominio.ObjetosDeValor.Embarcador.Fatura.FaturamentoLoteNotificacao> faturamentosPendentesNotificacao = repFaturamentoLote.BuscarFaturamentoAutomaticoPendenteNotificacao(false);
                if (faturamentosPendentesNotificacao != null && faturamentosPendentesNotificacao.Count > 0)
                {
                    List<long> codigosFaturamentoLote = faturamentosPendentesNotificacao.Select(c => c.Codigo).Distinct().ToList();
                    for (var i = 0; i < codigosFaturamentoLote.Count; i++)
                    {
                        List<int> faturas = faturamentosPendentesNotificacao.Where(c => c.Codigo == codigosFaturamentoLote[i]).Select(c => c.Fatura).Distinct().ToList();
                        if (GerarNotificacaoConclusaoFaturamento(faturas, codigosFaturamentoLote[i], unidadeTrabalho, stringConexao, tipoServicoMultisoftware))
                        {
                            Dominio.Entidades.Embarcador.Fatura.FaturamentoLote faturamentoLote = repFaturamentoLote.BuscarPorCodigo(codigosFaturamentoLote[i]);
                            faturamentoLote.NotificadoOperador = true;
                            repFaturamentoLote.Atualizar(faturamentoLote);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
            }
        }

        private static bool GerarCancelamentoLote(out string msgErro, Dominio.Entidades.Embarcador.Fatura.FaturamentoLote faturamentoLoteProcessar, Repositorio.UnitOfWork unidadeTrabalho, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            msgErro = string.Empty;
            Servicos.Embarcador.Fatura.Fatura servFatura = new Servicos.Embarcador.Fatura.Fatura(unidadeTrabalho);
            Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(stringConexao, null, tipoServicoMultisoftware, string.Empty);
            Repositorio.Embarcador.Fatura.FaturamentoLote repFaturamentoLote = new Repositorio.Embarcador.Fatura.FaturamentoLote(unidadeTrabalho);
            Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.FechamentoDiario repFechamentoDiario = new Repositorio.Embarcador.Financeiro.FechamentoDiario(unidadeTrabalho);

            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado();
            auditado.TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Usuario;
            auditado.OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema;
            auditado.Usuario = faturamentoLoteProcessar.Usuario;

            long codigoFaturamentoLote = faturamentoLoteProcessar.Codigo;
            string msg = "";

            if (faturamentoLoteProcessar.Faturas != null && faturamentoLoteProcessar.Faturas.Count > 0)
            {
                List<int> codigosFatura = faturamentoLoteProcessar.Faturas.Select(c => c.Codigo).ToList();

                foreach (var fatura in codigosFatura)
                {
                    Dominio.Entidades.Embarcador.Fatura.Fatura faturaCancelar = repFatura.BuscarPorCodigo(fatura);
                    Dominio.Entidades.Embarcador.Fatura.FaturamentoLote faturamentoLote = repFaturamentoLote.BuscarPorCodigo(codigoFaturamentoLote);
                    if (faturaCancelar.Situacao == SituacaoFatura.Fechado)
                    {
                        servFatura.CancelarTitulosBoletos(faturaCancelar.Codigo, unidadeTrabalho, auditado, faturamentoLote.Usuario.Empresa);

                        DateTime? dataUltimoFechamento = repFechamentoDiario.ObterUltimaDataFechamento();
                        DateTime dataCancelamento = faturaCancelar.DataFatura;

                        if (dataUltimoFechamento.HasValue && dataUltimoFechamento.Value >= dataCancelamento)
                            dataCancelamento = dataUltimoFechamento.Value.AddDays(1);

                        unidadeTrabalho.Start();

                        faturaCancelar.SituacaoNoCancelamento = faturaCancelar.Situacao;
                        faturaCancelar.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.EmCancelamento;
                        faturaCancelar.DataCancelamentoFatura = dataCancelamento;
                        faturaCancelar.MotivoCancelamento = faturamentoLote.Observacao;

                        servFatura.InserirLog(faturaCancelar, unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogFatura.CancelouFatura, faturamentoLote.Usuario);

                        Servicos.Auditoria.Auditoria.Auditar(auditado, faturaCancelar, null, "Cancelou a fatura pelo processo em lote.", unidadeTrabalho);

                        repFatura.Atualizar(faturaCancelar);

                        unidadeTrabalho.CommitChanges();
                    }
                    else
                        msg += " Fatura " + faturaCancelar.Numero.ToString() + " não estava com a situação Fechada.";

                    unidadeTrabalho.FlushAndClear();
                }
            }
            else
                msg = "Nenhuma fatura selecionada";

            faturamentoLoteProcessar = repFaturamentoLote.BuscarPorCodigo(codigoFaturamentoLote);
            faturamentoLoteProcessar.Situacao = SituacaoEnvioDocumentacaoLote.Finalizado;
            faturamentoLoteProcessar.MensagemRetorno = msg;
            repFaturamentoLote.Atualizar(faturamentoLoteProcessar);

            return true;
        }

        public static void ProcessarNotificacaoCancelamentoFaturamentoManual(Repositorio.UnitOfWork unidadeTrabalho, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            try
            {
                Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unidadeTrabalho);
                Repositorio.Embarcador.Fatura.FaturamentoLote repFaturamentoLote = new Repositorio.Embarcador.Fatura.FaturamentoLote(unidadeTrabalho);

                IList<Dominio.ObjetosDeValor.Embarcador.Fatura.FaturamentoLoteNotificacao> faturamentosPendentesNotificacao = repFaturamentoLote.BuscarCancelamentoFaturamentoManualPendenteNotificacao();
                if (faturamentosPendentesNotificacao != null && faturamentosPendentesNotificacao.Count > 0)
                {
                    List<int> codigosFaturas = faturamentosPendentesNotificacao.Select(c => c.Fatura).Distinct().ToList();
                    for (var i = 0; i < codigosFaturas.Count; i++)
                    {
                        List<int> codigosFatura = new List<int>();
                        codigosFatura.Add(codigosFaturas[i]);
                        GerarNotificacaoConclusaoCancelamento(codigosFatura, 0, unidadeTrabalho, stringConexao, tipoServicoMultisoftware);

                        Dominio.Entidades.Embarcador.Fatura.Fatura fatura = repFatura.BuscarPorCodigo(codigosFaturas[i]);
                        fatura.NotificadoOperador = true;
                        repFatura.Atualizar(fatura);
                    }
                }
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
            }
        }

        public static void ProcessarNotificacaoCancelamentoFaturamentoLote(Repositorio.UnitOfWork unidadeTrabalho, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            try
            {
                Repositorio.Embarcador.Fatura.FaturamentoLote repFaturamentoLote = new Repositorio.Embarcador.Fatura.FaturamentoLote(unidadeTrabalho);

                IList<Dominio.ObjetosDeValor.Embarcador.Fatura.FaturamentoLoteNotificacao> faturamentosPendentesNotificacao = repFaturamentoLote.BuscarCancelamentoFaturamentoPendenteNotificacao();
                if (faturamentosPendentesNotificacao != null && faturamentosPendentesNotificacao.Count > 0)
                {
                    List<long> codigosFaturamentoLote = faturamentosPendentesNotificacao.Select(c => c.Codigo).Distinct().ToList();
                    for (var i = 0; i < codigosFaturamentoLote.Count; i++)
                    {
                        var lista = faturamentosPendentesNotificacao.Where(c => c.Codigo == codigosFaturamentoLote[i]).ToList();
                        List<double> cnpjsPessoas = lista.Where(p => (double)p.Cliente > 0).Select(p => (double)p.Cliente).Distinct().ToList();
                        List<int> codigosGrupoPessoas = lista.Where(p => (int)p.Grupo > 0).Select(p => (int)p.Grupo).Distinct().ToList();

                        if (cnpjsPessoas != null && cnpjsPessoas.Count > 0)
                        {
                            foreach (var cnpjPessoa in cnpjsPessoas)
                            {
                                if (cnpjPessoa > 0)
                                {
                                    List<int> codigosFatura = lista.Where(p => (double)p.Cliente == cnpjPessoa).Select(p => (int)p.Fatura).Distinct().ToList();
                                    GerarNotificacaoConclusaoCancelamento(codigosFatura, codigosFaturamentoLote[i], unidadeTrabalho, stringConexao, tipoServicoMultisoftware);
                                }
                            }
                        }
                        if (codigosGrupoPessoas != null && codigosGrupoPessoas.Count > 0)
                        {
                            foreach (var codigoGrupoPessoa in codigosGrupoPessoas)
                            {
                                if (codigoGrupoPessoa > 0)
                                {
                                    List<int> codigosFatura = lista.Where(p => (int)p.Grupo == codigoGrupoPessoa).Select(p => (int)p.Fatura).Distinct().ToList();
                                    GerarNotificacaoConclusaoCancelamento(codigosFatura, codigosFaturamentoLote[i], unidadeTrabalho, stringConexao, tipoServicoMultisoftware);
                                }
                            }
                        }


                        Dominio.Entidades.Embarcador.Fatura.FaturamentoLote faturamentoLote = repFaturamentoLote.BuscarPorCodigo(codigosFaturamentoLote[i]);
                        faturamentoLote.NotificadoOperador = true;
                        faturamentoLote.Situacao = SituacaoEnvioDocumentacaoLote.Finalizado;
                        repFaturamentoLote.Atualizar(faturamentoLote);
                    }
                }
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
            }
        }

        public static DateTime RetornarProximaDataValida(DateTime dataAFaturar, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (dataAFaturar.DayOfWeek == DayOfWeek.Saturday)
            {
                dataAFaturar = dataAFaturar.AddDays(2);
                dataAFaturar = Utilidades.Conversor.ChangeTime(dataAFaturar, 8, 0, 0, 0);
            }
            else if (dataAFaturar.DayOfWeek == DayOfWeek.Sunday)
            {
                dataAFaturar = dataAFaturar.AddDays(1);
                dataAFaturar = Utilidades.Conversor.ChangeTime(dataAFaturar, 8, 0, 0, 0);
            }

            bool dataValida = true;
            Configuracoes.Feriado servicoFeriado = new Configuracoes.Feriado(unidadeDeTrabalho);

            while (dataValida)
            {
                if (dataAFaturar.DayOfWeek == DayOfWeek.Saturday || dataAFaturar.DayOfWeek == DayOfWeek.Sunday)
                {
                    dataAFaturar = dataAFaturar.AddDays(1);
                    dataAFaturar = Utilidades.Conversor.ChangeTime(dataAFaturar, 8, 0, 0, 0);
                }
                else if (servicoFeriado.VerificarSePossuiFeriado(dataAFaturar))
                {
                    dataAFaturar = dataAFaturar.AddDays(1);
                    dataAFaturar = Utilidades.Conversor.ChangeTime(dataAFaturar, 8, 0, 0, 0);
                }
                else
                    dataValida = false;
            }

            return dataAFaturar;
        }

        public static DateTime RetornarDataFaturar(DateTime dataSaidaNavioOriginal, int quantidadeHoras, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            DateTime dataSaidaNavio = RetornarProximaDataValida(dataSaidaNavioOriginal, unidadeDeTrabalho);
            while (quantidadeHoras > 0)
            {
                if (dataSaidaNavio.Hour >= 17)
                {
                    dataSaidaNavio = dataSaidaNavio.AddDays(1);
                    dataSaidaNavio = Utilidades.Conversor.ChangeTime(dataSaidaNavio, 8, 0, 0, 0);
                    dataSaidaNavio = RetornarProximaDataValida(dataSaidaNavio, unidadeDeTrabalho);
                }
                else if (dataSaidaNavio.Hour < 8)
                {
                    dataSaidaNavio = Utilidades.Conversor.ChangeTime(dataSaidaNavio, 8, 0, 0, 0);
                    dataSaidaNavio = RetornarProximaDataValida(dataSaidaNavio, unidadeDeTrabalho);
                }
                else
                {
                    var dataLimiteFatura = new DateTime(dataSaidaNavio.Year, dataSaidaNavio.Month, dataSaidaNavio.Day, 17, 00, 00);
                    TimeSpan tsDiferenca = dataSaidaNavio - dataLimiteFatura;
                    quantidadeHoras += (int)tsDiferenca.TotalHours;
                    if (quantidadeHoras == 0)
                    {
                        dataSaidaNavio = RetornarProximaDataValida(dataLimiteFatura, unidadeDeTrabalho);
                    }
                    else
                    {
                        dataSaidaNavio = dataSaidaNavio.AddDays(1);
                        dataSaidaNavio = Utilidades.Conversor.ChangeTime(dataSaidaNavio, 8, 0, 0, 0);
                        dataSaidaNavio = RetornarProximaDataValida(dataSaidaNavio, unidadeDeTrabalho);
                    }
                    if (quantidadeHoras < 9)
                    {
                        dataSaidaNavio = dataSaidaNavio.AddHours(quantidadeHoras);
                        quantidadeHoras = 0;
                    }
                }
            };

            return dataSaidaNavio;
        }

        public static bool InserirFaturaPorCTe(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string stringConexao, Dominio.Entidades.ConhecimentoDeTransporteEletronico CTe, string observacao, DateTime? dataInicial, DateTime? dataFinal, int idEmpresa, Dominio.Entidades.Usuario usuario, ref string msgErro)
        {

            Servicos.Embarcador.Fatura.Fatura servFatura = new Servicos.Embarcador.Fatura.Fatura(unitOfWork);
            Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(stringConexao, null, tipoServicoMultisoftware, string.Empty);

            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);
            Repositorio.Embarcador.Fatura.FaturaLoteCTe repFaturaLoteCTe = new Repositorio.Embarcador.Fatura.FaturaLoteCTe(unitOfWork);
            Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo(0);
            Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(idEmpresa);

            unitOfWork.Start();

            Dominio.Entidades.Embarcador.Fatura.Fatura fatura = new Dominio.Entidades.Embarcador.Fatura.Fatura();

            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado();
            auditado.TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Usuario;
            auditado.OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema;
            auditado.Usuario = usuario;
            auditado.Empresa = empresa;

            if (dataInicial.HasValue && dataInicial.Value > DateTime.MinValue)
                fatura.DataInicial = dataInicial;
            else
                fatura.DataInicial = null;
            if (dataFinal.HasValue && dataFinal.Value > DateTime.MinValue)
                fatura.DataFinal = dataFinal;
            else
                fatura.DataFinal = null;
            fatura.DataFatura = DateTime.Now.Date;
            fatura.NovoModelo = true;
            fatura.Etapa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFatura.LancandoCargas;
            fatura.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.EmAntamento;
            fatura.Observacao = observacao;
            fatura.Usuario = usuario;
            fatura.TipoPessoa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa.GrupoPessoa;
            fatura.GerarDocumentosAutomaticamente = true;

            fatura.AliquotaICMS = null;
            fatura.TipoCarga = null;
            fatura.Cliente = CTe.Tomador?.Cliente;
            fatura.IETomador = CTe.Tomador?.IE_RG;
            fatura.GrupoPessoas = CTe.Tomador?.GrupoPessoas;
            fatura.TipoOperacao = CTe.TipoOperacao;
            fatura.Transportador = null;
            fatura.CTe = CTe;
            fatura.Carga = CTe.CargaCTes?.FirstOrDefault()?.Carga;

            fatura.TerminalOrigem = CTe.TerminalOrigem;
            fatura.TerminalDestino = CTe.TerminalDestino;
            fatura.Origem = CTe.LocalidadeInicioPrestacao;
            fatura.Destino = CTe.LocalidadeTerminoPrestacao;
            fatura.NumeroBooking = CTe.NumeroBooking;
            fatura.GeradoPorFaturaLote = true;

            fatura.ImprimeObservacaoFatura = false;
            fatura.Total = 0;
            fatura.Numero = 0;
            fatura.DataFatura = DateTime.Now.Date;
            fatura.DataCancelamentoFatura = null;

            fatura.TipoCTe = CTe.TipoCTE;
            fatura.GeradoPorFaturaLote = true;

            if (fatura.Carga != null && fatura.Carga.CargaTakeOrPay)
            {
                fatura.ImprimeObservacaoFatura = true;
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPrimeiraPorCarga(fatura.Carga.Codigo);

                if (cargaPedido?.TipoPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.DemurrageCabotagem || cargaPedido?.TipoPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.DetentionCabotagem)
                {
                    fatura.ObservacaoFatura += "Navio/Viagem/Direção: " + (fatura.Carga.PedidoViagemNavio?.Descricao ?? "") + " \n";
                    fatura.ObservacaoFatura += "Porto de Origem: " + (fatura.Carga.PortoOrigem?.Descricao ?? "") + "\n";
                    fatura.ObservacaoFatura += "Porto de Destino:" + (fatura.Carga.PortoDestino?.Descricao ?? "") + "\n";
                    fatura.ObservacaoFatura += "Tipo Proposta: " + (cargaPedido?.TipoPropostaMultimodal.ObterDescricao()) + "\n" + "\n";
                }
                else if (cargaPedido?.TipoPropostaMultimodal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.FaturamentoContabilidade)
                {
                    int qtdDisponibilizadas = repCargaPedido.BuscarQuantidadeDisponibilizadas(fatura.Carga.Codigo);
                    int qtdNaoEmbarcadas = repCargaPedido.BuscarQuantidadeNaoEmbarcadas(fatura.Carga.Codigo);

                    fatura.ObservacaoFatura = "Fatura de Penalidade Contratual \n\n";
                    fatura.ObservacaoFatura += "Navio/Viagem/Direção: " + (fatura.Carga.PedidoViagemNavio?.Descricao ?? "") + " \n";
                    fatura.ObservacaoFatura += "Porto de Origem: " + (fatura.Carga.PortoOrigem?.Descricao ?? "") + "\n";
                    fatura.ObservacaoFatura += "Porto de Destino: " + (fatura.Carga.PortoDestino?.Descricao ?? "") + "\n";
                    fatura.ObservacaoFatura += "Quantidade de unidades disponibilizadas: " + (Utilidades.String.OnlyNumbers(qtdDisponibilizadas.ToString("n0"))) + " \n";
                    fatura.ObservacaoFatura += "Quantidade de unidades não embarcadas: " + (Utilidades.String.OnlyNumbers(qtdNaoEmbarcadas.ToString("n0"))) + "\n";
                }
                else
                    fatura.FaturaPropostaFaturamento = true;

                if (!string.IsNullOrWhiteSpace(fatura.Carga.ObservacaoParaFaturamento))
                {
                    if (!string.IsNullOrWhiteSpace(fatura.ObservacaoFatura))
                        fatura.ObservacaoFatura += "\n\n";
                    fatura.ObservacaoFatura += fatura.Carga.ObservacaoParaFaturamento;
                }
            }

            if (configuracaoTMS.GerarNumeracaoFaturaAnual)
            {
                int anoAtual = DateTime.Now.Year;
                fatura.ControleNumeracao = repFatura.BuscarProximoControleNumeracao(anoAtual);
                anoAtual = (anoAtual % 100);
                if (fatura.ControleNumeracao == 1 || (fatura.ControleNumeracao < ((anoAtual * 1000000) + 1)))
                    fatura.ControleNumeracao = (anoAtual * 1000000) + 1;
            }
            else
                fatura.ControleNumeracao = repFatura.BuscarProximoControleNumeracao();
            List<string> emails;
            if (TomadorPossuiEmailConfigurado(fatura, unitOfWork, out emails))
            {
                repFatura.Inserir(fatura, auditado);

                if (CTe.Codigo > 0)
                {
                    Dominio.Entidades.Embarcador.Fatura.FaturaLoteCTe faturaLoteCTe = new Dominio.Entidades.Embarcador.Fatura.FaturaLoteCTe()
                    {
                        CTe = repCTe.BuscarPorCodigo(CTe.Codigo),
                        Fatura = fatura
                    };
                    repFaturaLoteCTe.Inserir(faturaLoteCTe);
                }

                Servicos.Auditoria.Auditoria.Auditar(auditado, fatura, null, "Gerou a Fatura por lote.", unitOfWork);

                if (usuario != null)
                    servFatura.InserirLog(fatura, unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogFatura.IniciouFatura, usuario);

                unitOfWork.CommitChanges();
            }
            else
            {
                unitOfWork.Rollback();

                try
                {
                    if (usuario != null)
                        serNotificacao.GerarNotificacao(usuario, 0, "Faturas/FaturaLote", string.Format(Localization.Resources.Configuracao.Fatura.NaoFoiPossivelEncontrarEmailConfiguradoGerarFaturaTomador, (fatura.Cliente?.Descricao ?? fatura.GrupoPessoas?.Descricao ?? "")), IconesNotificacao.atencao, TipoNotificacao.todas, tipoServicoMultisoftware, unitOfWork);

                    msgErro += ("Não foi possível encontrar nenhum e-mail configurado para gerar a fatura do tomador " + (fatura.Cliente?.Descricao ?? fatura.GrupoPessoas?.Descricao ?? ""));
                    return false;

                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex, "FaturamentoLote");
                    return false;
                }
            }
            return true;

        }

        private static bool GerarFaturamentoLote(out string msgErro, Dominio.Entidades.Embarcador.Fatura.FaturamentoLote faturamentoLote, Repositorio.UnitOfWork unidadeTrabalho, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            msgErro = string.Empty;
            Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(stringConexao, null, tipoServicoMultisoftware, string.Empty);
            Repositorio.Embarcador.Fatura.FaturamentoLoteCTe repFaturamentoLoteCTe = new Repositorio.Embarcador.Fatura.FaturamentoLoteCTe(unidadeTrabalho);

            List<int> codigosCTes = new List<int>();
            if (!faturamentoLote.ConsultarTodos)
                codigosCTes = repFaturamentoLoteCTe.BuscarCodigosCTes(faturamentoLote.Codigo);

            if (faturamentoLote.TipoPessoa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa.GrupoPessoa)
            {
                if (!ProcessarFaturaLoteGrupoPessoa(out List<int> faturas, ref msgErro, faturamentoLote.Usuario?.Codigo ?? 0,
                    faturamentoLote.GrupoPessoas?.Codigo ?? 0, faturamentoLote.Cliente?.CPF_CNPJ ?? 0, faturamentoLote.Observacao,
                    faturamentoLote.DataInicial, faturamentoLote.DataFinal, faturamentoLote.DataFatura, faturamentoLote.TipoOperacao?.Codigo ?? 0,
                    faturamentoLote.Origem?.Codigo ?? 0, faturamentoLote.Destino?.Codigo ?? 0, faturamentoLote.NumeroBooking, faturamentoLote.PedidoViagemNavio?.Codigo ?? 0,
                    faturamentoLote.TerminalOrigem?.Codigo ?? 0, faturamentoLote.TerminalDestino?.Codigo ?? 0, faturamentoLote.TipoPropostaMultimodal != null ? faturamentoLote.TipoPropostaMultimodal.ToList() : null, null,
                    unidadeTrabalho, stringConexao, tipoServicoMultisoftware, faturamentoLote.Empresa?.Codigo ?? 0, faturamentoLote.FaturamentoAutomatico, faturamentoLote.Carga?.Codigo ?? 0,
                    faturamentoLote.GerarFaturamentoParaClientesExclusivos, codigosCTes, faturamentoLote.TipoCTe))
                {
                    AdicionarFaturasFaturamentoLote(faturas, faturamentoLote.Codigo, unidadeTrabalho, false, msgErro);
                    if (faturamentoLote.Usuario != null)
                        serNotificacao.GerarNotificacao(faturamentoLote.Usuario, 0, "Faturas/FaturaLote", string.Format(Localization.Resources.Configuracao.Fatura.FaturamentoProcessadoVerificarRetorno, msgErro), IconesNotificacao.atencao, TipoNotificacao.todas, tipoServicoMultisoftware, unidadeTrabalho);
                    return false;
                }
                else
                    AdicionarFaturasFaturamentoLote(faturas, faturamentoLote.Codigo, unidadeTrabalho, true, msgErro);
            }
            else
            {
                if (!ProcessarFaturaLotePessoa(out List<int> fatuas, ref msgErro, faturamentoLote.Usuario?.Codigo ?? 0, faturamentoLote.GrupoPessoas?.Codigo ?? 0, faturamentoLote.Cliente?.CPF_CNPJ ?? 0, faturamentoLote.Observacao, faturamentoLote.DataInicial, faturamentoLote.DataFinal, faturamentoLote.DataFatura, faturamentoLote.TipoOperacao?.Codigo ?? 0, faturamentoLote.Origem?.Codigo ?? 0, faturamentoLote.Destino?.Codigo ?? 0, faturamentoLote.NumeroBooking, faturamentoLote.PedidoViagemNavio?.Codigo ?? 0, faturamentoLote.TerminalOrigem?.Codigo ?? 0, faturamentoLote.TerminalDestino?.Codigo ?? 0, faturamentoLote.TipoPropostaMultimodal != null ? faturamentoLote.TipoPropostaMultimodal.ToList() : null, null, unidadeTrabalho, stringConexao, tipoServicoMultisoftware, faturamentoLote.Empresa?.Codigo ?? 0, faturamentoLote.FaturamentoAutomatico, faturamentoLote.Carga?.Codigo ?? 0, faturamentoLote.GerarFaturamentoParaClientesExclusivos, codigosCTes, faturamentoLote.TipoCTe))
                {
                    AdicionarFaturasFaturamentoLote(fatuas, faturamentoLote.Codigo, unidadeTrabalho, false, msgErro);
                    if (faturamentoLote.Usuario != null)
                        serNotificacao.GerarNotificacao(faturamentoLote.Usuario, 0, "Faturas/FaturaLote", string.Format(Localization.Resources.Configuracao.Fatura.FaturamentoProcessadoVerificarRetorno, msgErro), IconesNotificacao.atencao, TipoNotificacao.todas, tipoServicoMultisoftware, unidadeTrabalho);
                    return false;
                }
                else
                    AdicionarFaturasFaturamentoLote(fatuas, faturamentoLote.Codigo, unidadeTrabalho, true, msgErro);
            }

            return true;
        }

        private static bool AdicionarFaturasFaturamentoLote(List<int> fatuas, long codigoFaturamentoLote, Repositorio.UnitOfWork unitOfWork, bool sucesso, string msg)
        {
            Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);
            Repositorio.Embarcador.Fatura.FaturamentoLote repFaturamentoLote = new Repositorio.Embarcador.Fatura.FaturamentoLote(unitOfWork);

            Dominio.Entidades.Embarcador.Fatura.FaturamentoLote faturamentoLote = repFaturamentoLote.BuscarPorCodigo(codigoFaturamentoLote);
            if (fatuas != null && fatuas.Count > 0)
            {
                if (faturamentoLote.Faturas == null)
                    faturamentoLote.Faturas = new List<Dominio.Entidades.Embarcador.Fatura.Fatura>();
                for (int i = 0; i < fatuas.Count; i++)
                {
                    if (fatuas[i] > 0)
                        faturamentoLote.Faturas.Add(repFatura.BuscarPorCodigo(fatuas[i]));
                }
            }

            faturamentoLote.Situacao = sucesso ? SituacaoEnvioDocumentacaoLote.Finalizado : SituacaoEnvioDocumentacaoLote.Falha;
            faturamentoLote.MensagemRetorno = msg;
            repFaturamentoLote.Atualizar(faturamentoLote);
            return true;
        }

        private static bool ProcessarFaturaLoteGrupoPessoa(out List<int> faturas, ref string msgErro, int codigoUsuario, int codigoGrupoPessoaPesquisa, double cnpjPessoaPesquisa, string observacao, DateTime? dataInicial, DateTime? dataFinal, DateTime dataFatura, int codigoTipoOperacao, int codigoOrigem, int codigoDestino, string numeroBooking, int codigoPedidoViagemDirecao, int codigoTerminalOrigem, int codigoTerminalDestino, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal> tipoPropostaMultimodal, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal> tiposPropostasMultimodal, Repositorio.UnitOfWork unitOfWork, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, int codigoEmpresa, bool faturamentoAutomatico, int codigoCarga, bool gerarFaturamentoParaClientesExclusivos, List<int> codigosCTes, Dominio.Enumeradores.TipoCTE? tipoCTe)
        {
            faturas = new List<int>();
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
            Repositorio.Embarcador.Configuracoes.AcordoFaturamentoCliente repAcordoFaturamento = new Repositorio.Embarcador.Configuracoes.AcordoFaturamentoCliente(unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

            List<int> codigosGrupoSemEmail = new List<int>();
            List<double> cnpjsTomadorSemEmail = new List<double>();

            Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
            Dominio.ObjetosDeValor.Embarcador.Fatura.FiltroDocumentosParaFatura filtros = new Dominio.ObjetosDeValor.Embarcador.Fatura.FiltroDocumentosParaFatura()
            {
                CodigoGrupoPessoas = codigoGrupoPessoaPesquisa,
                CPFCNPJTomador = cnpjPessoaPesquisa,
                DataInicial = dataInicial,
                DataFinal = dataFinal,
                TipoOperacao = codigoTipoOperacao,
                PedidoViagemNavio = codigoPedidoViagemDirecao,
                TerminalOrigem = codigoTerminalOrigem,
                TerminalDestino = codigoTerminalDestino,
                Origem = codigoOrigem,
                Destino = codigoDestino,
                NumeroBooking = numeroBooking,
                TipoPropostaMultimodal = tipoPropostaMultimodal,
                TiposPropostasMultimodal = tiposPropostasMultimodal,
                CodigoCarga = codigoCarga,
                ApenasFaturaExclusiva = gerarFaturamentoParaClientesExclusivos,
                CodigosCTes = codigosCTes,
                TipoCTe = tipoCTe.HasValue ? tipoCTe.Value : Dominio.Enumeradores.TipoCTE.Normal
            };

            List<int> codigosGrupoPessoaParaFatura = repDocumentoFaturamento.ConsultarCodigosGrupoPessoaParaFatura(filtros);

            if (codigosGrupoPessoaParaFatura != null && codigosGrupoPessoaParaFatura.Count > 0)
            {
                codigosGrupoPessoaParaFatura = codigosGrupoPessoaParaFatura.Distinct().ToList();

                foreach (var codigoGrupoPessoa in codigosGrupoPessoaParaFatura)
                {
                    filtros = new Dominio.ObjetosDeValor.Embarcador.Fatura.FiltroDocumentosParaFatura()
                    {
                        CodigoGrupoPessoas = codigoGrupoPessoa,
                        CPFCNPJTomador = cnpjPessoaPesquisa,
                        DataInicial = dataInicial,
                        DataFinal = dataFinal,
                        TipoOperacao = codigoTipoOperacao,
                        PedidoViagemNavio = codigoPedidoViagemDirecao,
                        TerminalOrigem = codigoTerminalOrigem,
                        TerminalDestino = codigoTerminalDestino,
                        Origem = codigoOrigem,
                        Destino = codigoDestino,
                        NumeroBooking = numeroBooking,
                        TipoPropostaMultimodal = tipoPropostaMultimodal,
                        TiposPropostasMultimodal = tiposPropostasMultimodal,
                        CodigoCarga = codigoCarga,
                        ApenasFaturaExclusiva = gerarFaturamentoParaClientesExclusivos,
                        CodigosCTes = codigosCTes,
                        TipoCTe = tipoCTe.HasValue ? tipoCTe.Value : Dominio.Enumeradores.TipoCTE.Normal
                    };
                    List<double> cnpjsTomadorGrupoPessoaParaFatura = repDocumentoFaturamento.ConsultarCodigosTomadorParaFatura(filtros);
                    if (cnpjsTomadorGrupoPessoaParaFatura != null && cnpjsTomadorGrupoPessoaParaFatura.Count > 0)
                    {
                        foreach (var cnpjTomador in cnpjsTomadorGrupoPessoaParaFatura)
                        {
                            filtros = new Dominio.ObjetosDeValor.Embarcador.Fatura.FiltroDocumentosParaFatura()
                            {
                                CodigoGrupoPessoas = codigoGrupoPessoa,
                                CPFCNPJTomador = cnpjTomador,
                                DataInicial = dataInicial,
                                DataFinal = dataFinal,
                                TipoOperacao = codigoTipoOperacao,
                                PedidoViagemNavio = codigoPedidoViagemDirecao,
                                TerminalOrigem = codigoTerminalOrigem,
                                TerminalDestino = codigoTerminalDestino,
                                Origem = codigoOrigem,
                                Destino = codigoDestino,
                                NumeroBooking = numeroBooking,
                                TipoPropostaMultimodal = tipoPropostaMultimodal,
                                TiposPropostasMultimodal = tiposPropostasMultimodal,
                                CodigoCarga = codigoCarga,
                                ApenasFaturaExclusiva = gerarFaturamentoParaClientesExclusivos,
                                CodigosCTes = codigosCTes,
                                TipoCTe = tipoCTe.HasValue ? tipoCTe.Value : Dominio.Enumeradores.TipoCTE.Normal
                            };
                            List<string> inscricaoTomadorGrupoPessoaParaFatura = repDocumentoFaturamento.ConsultarInscricaoTomadorParaFatura(filtros);

                            if (inscricaoTomadorGrupoPessoaParaFatura != null && inscricaoTomadorGrupoPessoaParaFatura.Count > 0)
                            {
                                foreach (var ieTomador in inscricaoTomadorGrupoPessoaParaFatura)
                                {
                                    Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoa = repGrupoPessoas.BuscarPorCodigo(codigoGrupoPessoa);
                                    Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorCodigo(codigoUsuario);

                                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAgrupamentoFatura tipoAgrupamentoFatura = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAgrupamentoFatura.Manual;

                                    if (grupoPessoa.TipoAgrupamentoFatura.HasValue)
                                        tipoAgrupamentoFatura = grupoPessoa.TipoAgrupamentoFatura.Value;

                                    Dominio.ObjetosDeValor.Embarcador.Fatura.FiltroDocumentosParaFatura filtrosGrupoPessoas = new Dominio.ObjetosDeValor.Embarcador.Fatura.FiltroDocumentosParaFatura()
                                    {
                                        CodigoGrupoPessoas = codigoGrupoPessoa,
                                        CPFCNPJTomador = cnpjTomador,
                                        IETomador = ieTomador,
                                        DataInicial = dataInicial,
                                        DataFinal = dataFinal,
                                        TipoOperacao = codigoTipoOperacao,
                                        PedidoViagemNavio = codigoPedidoViagemDirecao,
                                        TerminalOrigem = codigoTerminalOrigem,
                                        TerminalDestino = codigoTerminalDestino,
                                        Origem = codigoOrigem,
                                        Destino = codigoDestino,
                                        NumeroBooking = numeroBooking,
                                        TipoPropostaMultimodal = tipoPropostaMultimodal,
                                        TiposPropostasMultimodal = tiposPropostasMultimodal,
                                        CodigoCarga = codigoCarga,
                                        ApenasFaturaExclusiva = gerarFaturamentoParaClientesExclusivos,
                                        CodigosCTes = codigosCTes,
                                        TipoCTe = tipoCTe.HasValue ? tipoCTe.Value : Dominio.Enumeradores.TipoCTE.Normal
                                    };

                                    if (filtrosGrupoPessoas.CodigoCarga > 0)
                                        tipoAgrupamentoFatura = TipoAgrupamentoFatura.Manual;


                                    Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoCliente acordoFaturamento = repAcordoFaturamento.BuscarAcordoCliente(0, codigoGrupoPessoa);
                                    bool permiteGerarFatura = true;
                                    if (acordoFaturamento != null && faturamentoAutomatico)
                                    {
                                        if (!gerarFaturamentoParaClientesExclusivos && acordoFaturamento.FaturamentoPermissaoExclusivaCabotagem)
                                            permiteGerarFatura = false;
                                    }
                                    if (!permiteGerarFatura)
                                        continue; // Pula para o próximo grupo

                                    if (tipoAgrupamentoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAgrupamentoFatura.Manual)
                                    {
                                        msgErro += "";
                                        if (!InserirFatura(ieTomador, out int codigoGrupoSemEmail, out double cnpjTomadorSemEmail, out int codigoFatura, ref msgErro, 0, 0, "", "", 0, codigoGrupoPessoa, cnpjTomador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa.GrupoPessoa, observacao, dataInicial, dataFinal, dataFatura, codigoTipoOperacao, codigoOrigem, codigoDestino, numeroBooking, codigoPedidoViagemDirecao, codigoTerminalOrigem, codigoTerminalDestino, tipoPropostaMultimodal, unitOfWork, usuario, stringConexao, tipoServicoMultisoftware, empresa, codigoCarga, codigosCTes, gerarFaturamentoParaClientesExclusivos, tipoCTe))
                                        {
                                            codigosGrupoSemEmail.Add(codigoGrupoSemEmail);
                                            cnpjsTomadorSemEmail.Add(cnpjTomadorSemEmail);
                                            return false;
                                        }
                                        else
                                        {
                                            codigosGrupoSemEmail.Add(codigoGrupoSemEmail);
                                            cnpjsTomadorSemEmail.Add(cnpjTomadorSemEmail);
                                        }
                                        faturas.Add(codigoFatura);
                                    }
                                    else if (tipoAgrupamentoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAgrupamentoFatura.Booking)
                                    {
                                        List<string> numerosBookings = new List<string>();

                                        numerosBookings = repDocumentoFaturamento.ConsultarBookingsParaFatura(filtrosGrupoPessoas);

                                        if (numerosBookings != null && numerosBookings.Count > 0 && !string.IsNullOrWhiteSpace(numerosBookings[0]))
                                        {
                                            foreach (var numero in numerosBookings)
                                            {
                                                msgErro += "";
                                                if (!InserirFatura(ieTomador, out int codigoGrupoSemEmail, out double cnpjTomadorSemEmail, out int codigoFatura, ref msgErro, 0, 0, "", "", 0, codigoGrupoPessoa, cnpjTomador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa.GrupoPessoa, observacao, dataInicial, dataFinal, dataFatura, codigoTipoOperacao, codigoOrigem, codigoDestino, numero, codigoPedidoViagemDirecao, codigoTerminalOrigem, codigoTerminalDestino, tipoPropostaMultimodal, unitOfWork, usuario, stringConexao, tipoServicoMultisoftware, empresa, codigoCarga, codigosCTes, gerarFaturamentoParaClientesExclusivos, tipoCTe))
                                                {
                                                    codigosGrupoSemEmail.Add(codigoGrupoSemEmail);
                                                    cnpjsTomadorSemEmail.Add(cnpjTomadorSemEmail);
                                                    return false;
                                                }
                                                else
                                                {
                                                    codigosGrupoSemEmail.Add(codigoGrupoSemEmail);
                                                    cnpjsTomadorSemEmail.Add(cnpjTomadorSemEmail);
                                                }
                                                faturas.Add(codigoFatura);
                                            }
                                        }
                                        else
                                        {
                                            msgErro += " Não foi encontrado Booking disponível para realizar o faturamento";
                                            //return false;
                                        }
                                    }
                                    else if (tipoAgrupamentoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAgrupamentoFatura.Container)
                                    {
                                        List<int> codigosContainer = new List<int>();

                                        codigosContainer = repDocumentoFaturamento.ConsultarCodigosContainerParaFatura(filtrosGrupoPessoas);

                                        if (codigosContainer != null && codigosContainer.Count > 0)
                                        {
                                            foreach (var codigoContainer in codigosContainer)
                                            {
                                                msgErro += "";
                                                if (!InserirFatura(ieTomador, out int codigoGrupoSemEmail, out double cnpjTomadorSemEmail, out int codigoFatura, ref msgErro, 0, codigoContainer, "", "", 0, codigoGrupoPessoa, cnpjTomador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa.GrupoPessoa, observacao, dataInicial, dataFinal, dataFatura, codigoTipoOperacao, codigoOrigem, codigoDestino, numeroBooking, codigoPedidoViagemDirecao, codigoTerminalOrigem, codigoTerminalDestino, tipoPropostaMultimodal, unitOfWork, usuario, stringConexao, tipoServicoMultisoftware, empresa, codigoCarga, codigosCTes, gerarFaturamentoParaClientesExclusivos, tipoCTe))
                                                {
                                                    codigosGrupoSemEmail.Add(codigoGrupoSemEmail);
                                                    cnpjsTomadorSemEmail.Add(cnpjTomadorSemEmail);
                                                    return false;
                                                }
                                                else
                                                {
                                                    codigosGrupoSemEmail.Add(codigoGrupoSemEmail);
                                                    cnpjsTomadorSemEmail.Add(cnpjTomadorSemEmail);
                                                }
                                                faturas.Add(codigoFatura);
                                            }
                                        }
                                        else
                                        {
                                            msgErro += "Não foi encontrado Container disponível para realizar o faturamento";
                                        }
                                    }
                                    else if (tipoAgrupamentoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAgrupamentoFatura.CTe)
                                    {
                                        List<int> codigos = new List<int>();

                                        codigos = repDocumentoFaturamento.ConsultarCTesParaFatura(filtrosGrupoPessoas);

                                        if (codigos != null && codigos.Count > 0)
                                        {
                                            foreach (var codigo in codigos)
                                            {
                                                msgErro += "";
                                                if (!InserirFatura(ieTomador, out int codigoGrupoSemEmail, out double cnpjTomadorSemEmail, out int codigoFatura, ref msgErro, 0, 0, "", "", codigo, codigoGrupoPessoa, cnpjTomador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa.GrupoPessoa, observacao, dataInicial, dataFinal, dataFatura, codigoTipoOperacao, codigoOrigem, codigoDestino, numeroBooking, codigoPedidoViagemDirecao, codigoTerminalOrigem, codigoTerminalDestino, tipoPropostaMultimodal, unitOfWork, usuario, stringConexao, tipoServicoMultisoftware, empresa, codigoCarga, codigosCTes, gerarFaturamentoParaClientesExclusivos, tipoCTe))
                                                {
                                                    codigosGrupoSemEmail.Add(codigoGrupoSemEmail);
                                                    cnpjsTomadorSemEmail.Add(cnpjTomadorSemEmail);
                                                    return false;
                                                }
                                                else
                                                {
                                                    codigosGrupoSemEmail.Add(codigoGrupoSemEmail);
                                                    cnpjsTomadorSemEmail.Add(cnpjTomadorSemEmail);
                                                }
                                                faturas.Add(codigoFatura);
                                            }
                                        }
                                        else
                                        {
                                            msgErro += " Não foi encontrado Conhecimento disponível para realizar o faturamento";
                                        }
                                    }
                                    else if (tipoAgrupamentoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAgrupamentoFatura.NavioViagemDirecaoPOL)
                                    {
                                        List<int> codigos = new List<int>();

                                        codigos = repDocumentoFaturamento.ConsultarCodigosPedidoNavioViagemParaFatura(filtrosGrupoPessoas);

                                        if (codigos != null && codigos.Count > 0)
                                        {
                                            foreach (var codigo in codigos)
                                            {
                                                msgErro += "";
                                                if (!InserirFatura(ieTomador, out int codigoGrupoSemEmail, out double cnpjTomadorSemEmail, out int codigoFatura, ref msgErro, 0, 0, "", "", 0, codigoGrupoPessoa, cnpjTomador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa.GrupoPessoa, observacao, dataInicial, dataFinal, dataFatura, codigoTipoOperacao, codigoOrigem, codigoDestino, numeroBooking, codigo, codigoTerminalOrigem, codigoTerminalDestino, tipoPropostaMultimodal, unitOfWork, usuario, stringConexao, tipoServicoMultisoftware, empresa, codigoCarga, codigosCTes, gerarFaturamentoParaClientesExclusivos, tipoCTe))
                                                {
                                                    codigosGrupoSemEmail.Add(codigoGrupoSemEmail);
                                                    cnpjsTomadorSemEmail.Add(cnpjTomadorSemEmail);
                                                    return false;
                                                }
                                                else
                                                {
                                                    codigosGrupoSemEmail.Add(codigoGrupoSemEmail);
                                                    cnpjsTomadorSemEmail.Add(cnpjTomadorSemEmail);
                                                }
                                                faturas.Add(codigoFatura);
                                            }
                                        }
                                        else
                                        {
                                            msgErro += " Não foi encontrado Navio/Viagem/Direção disponível para realizar o faturamento";
                                        }
                                    }
                                    else if (tipoAgrupamentoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAgrupamentoFatura.NavioViagemDirecaoPOLPOD)
                                    {
                                        List<int> codigos = new List<int>();

                                        codigos = repDocumentoFaturamento.ConsultarCodigosPedidoNavioViagemParaFatura(filtrosGrupoPessoas);

                                        if (codigos != null && codigos.Count > 0)
                                        {
                                            foreach (var codigo in codigos)
                                            {
                                                List<int> codigosTerminalDestino = new List<int>();

                                                Dominio.ObjetosDeValor.Embarcador.Fatura.FiltroDocumentosParaFatura filtrosTerminalDestino = new Dominio.ObjetosDeValor.Embarcador.Fatura.FiltroDocumentosParaFatura()
                                                {
                                                    CodigoGrupoPessoas = codigoGrupoPessoa,
                                                    DataInicial = dataInicial,
                                                    DataFinal = dataFinal,
                                                    TipoOperacao = codigoTipoOperacao,
                                                    PedidoViagemNavio = codigo,
                                                    TerminalOrigem = codigoTerminalOrigem,
                                                    TerminalDestino = codigoTerminalDestino,
                                                    Origem = codigoOrigem,
                                                    Destino = codigoDestino,
                                                    NumeroBooking = numeroBooking,
                                                    TipoPropostaMultimodal = tipoPropostaMultimodal,
                                                    TiposPropostasMultimodal = tiposPropostasMultimodal,
                                                    CodigoCarga = codigoCarga,
                                                    ApenasFaturaExclusiva = gerarFaturamentoParaClientesExclusivos,
                                                    CodigosCTes = codigosCTes,
                                                    TipoCTe = tipoCTe.HasValue ? tipoCTe.Value : Dominio.Enumeradores.TipoCTE.Normal
                                                };

                                                codigosTerminalDestino = repDocumentoFaturamento.ConsultarCodigosTerminalDestinoParaFatura(filtrosTerminalDestino);

                                                if (codigosTerminalDestino != null && codigosTerminalDestino.Count > 0)
                                                {
                                                    foreach (var codTerminalDestino in codigosTerminalDestino)
                                                    {
                                                        msgErro += "";
                                                        if (!InserirFatura(ieTomador, out int codigoGrupoSemEmail, out double cnpjTomadorSemEmail, out int codigoFatura, ref msgErro, 0, 0, "", "", 0, codigoGrupoPessoa, cnpjTomador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa.GrupoPessoa, observacao, dataInicial, dataFinal, dataFatura, codigoTipoOperacao, codigoOrigem, codigoDestino, numeroBooking, codigo, codigoTerminalOrigem, codTerminalDestino, tipoPropostaMultimodal, unitOfWork, usuario, stringConexao, tipoServicoMultisoftware, empresa, codigoCarga, codigosCTes, gerarFaturamentoParaClientesExclusivos, tipoCTe))
                                                        {
                                                            codigosGrupoSemEmail.Add(codigoGrupoSemEmail);
                                                            cnpjsTomadorSemEmail.Add(cnpjTomadorSemEmail);
                                                            return false;
                                                        }
                                                        else
                                                        {
                                                            codigosGrupoSemEmail.Add(codigoGrupoSemEmail);
                                                            cnpjsTomadorSemEmail.Add(cnpjTomadorSemEmail);
                                                        }
                                                        faturas.Add(codigoFatura);
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            msgErro += " Não foi encontrado CT-e disponível para realizar o faturamento, verifique se o mesmo já não foi faturado.";
                                        }
                                    }
                                    else if (tipoAgrupamentoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAgrupamentoFatura.NumeroControleCliente)
                                    {
                                        List<string> codigos = new List<string>();

                                        codigos = repDocumentoFaturamento.ConsultarNumeroControleClienteParaFatura(filtrosGrupoPessoas);

                                        if (codigos != null && codigos.Count > 0 && !string.IsNullOrWhiteSpace(codigos[0]))
                                        {
                                            foreach (var codigo in codigos)
                                            {
                                                msgErro += "";
                                                if (!InserirFatura(ieTomador, out int codigoGrupoSemEmail, out double cnpjTomadorSemEmail, out int codigoFatura, ref msgErro, 0, 0, codigo, "", 0, codigoGrupoPessoa, cnpjTomador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa.GrupoPessoa, observacao, dataInicial, dataFinal, dataFatura, codigoTipoOperacao, codigoOrigem, codigoDestino, numeroBooking, codigoPedidoViagemDirecao, codigoTerminalOrigem, codigoTerminalDestino, tipoPropostaMultimodal, unitOfWork, usuario, stringConexao, tipoServicoMultisoftware, empresa, codigoCarga, codigosCTes, gerarFaturamentoParaClientesExclusivos, tipoCTe))
                                                {
                                                    codigosGrupoSemEmail.Add(codigoGrupoSemEmail);
                                                    cnpjsTomadorSemEmail.Add(cnpjTomadorSemEmail);
                                                    return false;
                                                }
                                                else
                                                {
                                                    codigosGrupoSemEmail.Add(codigoGrupoSemEmail);
                                                    cnpjsTomadorSemEmail.Add(cnpjTomadorSemEmail);
                                                }
                                                faturas.Add(codigoFatura);
                                            }
                                        }
                                        else
                                        {
                                            msgErro += " Não foi encontrado Número de Controle disponível para realizar o faturamento";
                                        }
                                    }
                                    else if (tipoAgrupamentoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAgrupamentoFatura.NumeroReferenciaEDI)
                                    {
                                        List<string> codigos = new List<string>();

                                        codigos = repDocumentoFaturamento.ConsultarNumeroReferenciaEDIParaFatura(filtrosGrupoPessoas);

                                        if (codigos != null && codigos.Count > 0 && !string.IsNullOrWhiteSpace(codigos[0]))
                                        {
                                            foreach (var codigo in codigos)
                                            {
                                                msgErro += "";
                                                if (!InserirFatura(ieTomador, out int codigoGrupoSemEmail, out double cnpjTomadorSemEmail, out int codigoFatura, ref msgErro, 0, 0, "", codigo, 0, codigoGrupoPessoa, cnpjTomador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa.GrupoPessoa, observacao, dataInicial, dataFinal, dataFatura, codigoTipoOperacao, codigoOrigem, codigoDestino, numeroBooking, codigoPedidoViagemDirecao, codigoTerminalOrigem, codigoTerminalDestino, tipoPropostaMultimodal, unitOfWork, usuario, stringConexao, tipoServicoMultisoftware, empresa, codigoCarga, codigosCTes, gerarFaturamentoParaClientesExclusivos, tipoCTe))
                                                {
                                                    codigosGrupoSemEmail.Add(codigoGrupoSemEmail);
                                                    cnpjsTomadorSemEmail.Add(cnpjTomadorSemEmail);
                                                    return false;
                                                }
                                                else
                                                {
                                                    codigosGrupoSemEmail.Add(codigoGrupoSemEmail);
                                                    cnpjsTomadorSemEmail.Add(cnpjTomadorSemEmail);
                                                }
                                                faturas.Add(codigoFatura);
                                            }
                                        }
                                        else
                                        {
                                            msgErro += " Não foi encontrado Número de Referencia de EDI disponível para realizar o faturamento";
                                        }
                                    }

                                    unitOfWork.FlushAndClear();
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                msgErro = "Não foi encontrado nenhum Grupo de Pessoa com pendência de faturamento";
                return false;
            }

            if (codigosGrupoSemEmail.Count > 0 || cnpjsTomadorSemEmail.Count > 0)
                GerarNotificacaoTomadorSemEmail(codigosGrupoSemEmail, cnpjsTomadorSemEmail, codigoPedidoViagemDirecao, codigoTerminalOrigem, unitOfWork);

            return true;
        }

        private static bool ProcessarFaturaLotePessoa(out List<int> fatuas, ref string msgErro, int codigoUsuario, int codigoGrupoPessoaPesquisa, double cnpjPessoaPesquisa, string observacao, DateTime? dataInicial, DateTime? dataFinal, DateTime dataFatura, int codigoTipoOperacao, int codigoOrigem, int codigoDestino, string numeroBooking, int codigoPedidoViagemDirecao, int codigoTerminalOrigem, int codigoTerminalDestino, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal> tipoPropostaMultimodal, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal> tiposPropostasMultimodal, Repositorio.UnitOfWork unitOfWork, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, int codigoEmpresa, bool faturamentoAutomatico, int codigoCarga, bool gerarFaturamentoParaClientesExclusivos, List<int> codigosCTes, Dominio.Enumeradores.TipoCTE? tipoCTe)
        {
            fatuas = new List<int>();
            msgErro = "";
            Repositorio.Embarcador.Configuracoes.AcordoFaturamentoCliente repAcordoFaturamento = new Repositorio.Embarcador.Configuracoes.AcordoFaturamentoCliente(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);

            List<int> codigosGrupoSemEmail = new List<int>();
            List<double> cnpjsTomadorSemEmail = new List<double>();

            Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
            Dominio.ObjetosDeValor.Embarcador.Fatura.FiltroDocumentosParaFatura filtros = new Dominio.ObjetosDeValor.Embarcador.Fatura.FiltroDocumentosParaFatura()
            {
                CodigoGrupoPessoas = codigoGrupoPessoaPesquisa,
                CPFCNPJTomador = cnpjPessoaPesquisa,
                DataInicial = dataInicial,
                DataFinal = dataFinal,
                TipoOperacao = codigoTipoOperacao,
                PedidoViagemNavio = codigoPedidoViagemDirecao,
                TerminalOrigem = codigoTerminalOrigem,
                TerminalDestino = codigoTerminalDestino,
                Origem = codigoOrigem,
                Destino = codigoDestino,
                NumeroBooking = numeroBooking,
                TipoPropostaMultimodal = tipoPropostaMultimodal,
                TiposPropostasMultimodal = tiposPropostasMultimodal,
                CodigoCarga = codigoCarga,
                ApenasFaturaExclusiva = gerarFaturamentoParaClientesExclusivos,
                CodigosCTes = codigosCTes,
                TipoCTe = tipoCTe.HasValue ? tipoCTe.Value : Dominio.Enumeradores.TipoCTE.Normal
            };

            List<double> cnpjsTomador = repDocumentoFaturamento.ConsultarCodigosTomadorParaFatura(filtros);

            if (cnpjsTomador != null && cnpjsTomador.Count > 0)
            {
                cnpjsTomador = cnpjsTomador.Distinct().ToList();

                foreach (var cnpjTomador in cnpjsTomador)
                {
                    Dominio.Entidades.Cliente tomador = repCliente.BuscarPorCPFCNPJ(cnpjTomador);
                    Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorCodigo(codigoUsuario);

                    Dominio.ObjetosDeValor.Embarcador.Fatura.FiltroDocumentosParaFatura filtrosTomador = new Dominio.ObjetosDeValor.Embarcador.Fatura.FiltroDocumentosParaFatura()
                    {
                        CPFCNPJTomador = cnpjTomador,
                        DataInicial = dataInicial,
                        DataFinal = dataFinal,
                        TipoOperacao = codigoTipoOperacao,
                        PedidoViagemNavio = codigoPedidoViagemDirecao,
                        TerminalOrigem = codigoTerminalOrigem,
                        TerminalDestino = codigoTerminalDestino,
                        Origem = codigoOrigem,
                        Destino = codigoDestino,
                        NumeroBooking = numeroBooking,
                        TipoPropostaMultimodal = tipoPropostaMultimodal,
                        TiposPropostasMultimodal = tiposPropostasMultimodal,
                        CodigoCarga = codigoCarga,
                        ApenasFaturaExclusiva = gerarFaturamentoParaClientesExclusivos,
                        CodigosCTes = codigosCTes,
                        TipoCTe = tipoCTe.HasValue ? tipoCTe.Value : Dominio.Enumeradores.TipoCTE.Normal
                    };

                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAgrupamentoFatura tipoAgrupamentoFatura = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAgrupamentoFatura.Manual;
                    if (tomador.NaoUsarConfiguracaoFaturaGrupo && tomador.TipoAgrupamentoFatura.HasValue)
                        tipoAgrupamentoFatura = tomador.TipoAgrupamentoFatura.Value;
                    else if (tomador.GrupoPessoas != null && tomador.GrupoPessoas.TipoAgrupamentoFatura.HasValue)
                        tipoAgrupamentoFatura = tomador.GrupoPessoas.TipoAgrupamentoFatura.Value;

                    Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoCliente acordoFaturamento = repAcordoFaturamento.BuscarAcordoCliente(cnpjTomador, 0);
                    bool permiteGerarFatura = true;
                    if (acordoFaturamento != null && faturamentoAutomatico)
                    {
                        if (!gerarFaturamentoParaClientesExclusivos && acordoFaturamento.FaturamentoPermissaoExclusivaCabotagem)
                            permiteGerarFatura = false;
                    }
                    if (!permiteGerarFatura)
                        continue; // Pula para o próximo grupo

                    if (filtrosTomador.CodigoCarga > 0)
                        tipoAgrupamentoFatura = TipoAgrupamentoFatura.Manual;

                    if (tipoAgrupamentoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAgrupamentoFatura.Manual)
                    {
                        msgErro += "";
                        if (!InserirFatura("", out int codigoGrupoSemEmail, out double cnpjTomadorSemEmail, out int codigoFatura, ref msgErro, 0, 0, "", "", 0, 0, cnpjTomador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa.Pessoa, observacao, dataInicial, dataFinal, dataFatura, codigoTipoOperacao, codigoOrigem, codigoDestino, numeroBooking, codigoPedidoViagemDirecao, codigoTerminalOrigem, codigoTerminalDestino, tipoPropostaMultimodal, unitOfWork, usuario, stringConexao, tipoServicoMultisoftware, empresa, codigoCarga, codigosCTes, gerarFaturamentoParaClientesExclusivos, tipoCTe))
                        {
                            codigosGrupoSemEmail.Add(codigoGrupoSemEmail);
                            cnpjsTomadorSemEmail.Add(cnpjTomadorSemEmail);
                            return false;
                        }
                        else
                        {
                            codigosGrupoSemEmail.Add(codigoGrupoSemEmail);
                            cnpjsTomadorSemEmail.Add(cnpjTomadorSemEmail);
                        }
                        fatuas.Add(codigoFatura);
                    }
                    else if (tipoAgrupamentoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAgrupamentoFatura.Booking)
                    {
                        List<string> numerosBookings = new List<string>();

                        numerosBookings = repDocumentoFaturamento.ConsultarBookingsParaFatura(filtrosTomador);

                        if (numerosBookings != null && numerosBookings.Count > 0 && !string.IsNullOrWhiteSpace(numerosBookings[0]))
                        {
                            foreach (var numero in numerosBookings)
                            {
                                msgErro += "";
                                if (!InserirFatura("", out int codigoGrupoSemEmail, out double cnpjTomadorSemEmail, out int codigoFatura, ref msgErro, 0, 0, "", "", 0, 0, cnpjTomador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa.Pessoa, observacao, dataInicial, dataFinal, dataFatura, codigoTipoOperacao, codigoOrigem, codigoDestino, numero, codigoPedidoViagemDirecao, codigoTerminalOrigem, codigoTerminalDestino, tipoPropostaMultimodal, unitOfWork, usuario, stringConexao, tipoServicoMultisoftware, empresa, codigoCarga, codigosCTes, gerarFaturamentoParaClientesExclusivos, tipoCTe))
                                {
                                    codigosGrupoSemEmail.Add(codigoGrupoSemEmail);
                                    cnpjsTomadorSemEmail.Add(cnpjTomadorSemEmail);
                                    return false;
                                }
                                else
                                {
                                    codigosGrupoSemEmail.Add(codigoGrupoSemEmail);
                                    cnpjsTomadorSemEmail.Add(cnpjTomadorSemEmail);
                                }
                                fatuas.Add(codigoFatura);
                            }
                        }
                        else
                        {
                            msgErro += " Não foi encontrado Booking disponível para realizar o faturamento";
                        }
                    }
                    else if (tipoAgrupamentoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAgrupamentoFatura.Container)
                    {
                        List<int> codigosContainer = new List<int>();

                        codigosContainer = repDocumentoFaturamento.ConsultarCodigosContainerParaFatura(filtrosTomador);

                        if (codigosContainer != null && codigosContainer.Count > 0)
                        {
                            foreach (var codigoContainer in codigosContainer)
                            {
                                msgErro += "";
                                if (!InserirFatura("", out int codigoGrupoSemEmail, out double cnpjTomadorSemEmail, out int codigoFatura, ref msgErro, 0, codigoContainer, "", "", 0, 0, cnpjTomador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa.Pessoa, observacao, dataInicial, dataFinal, dataFatura, codigoTipoOperacao, codigoOrigem, codigoDestino, numeroBooking, codigoPedidoViagemDirecao, codigoTerminalOrigem, codigoTerminalDestino, tipoPropostaMultimodal, unitOfWork, usuario, stringConexao, tipoServicoMultisoftware, empresa, codigoCarga, codigosCTes, gerarFaturamentoParaClientesExclusivos, tipoCTe))
                                {
                                    codigosGrupoSemEmail.Add(codigoGrupoSemEmail);
                                    cnpjsTomadorSemEmail.Add(cnpjTomadorSemEmail);
                                    return false;
                                }
                                else
                                {
                                    codigosGrupoSemEmail.Add(codigoGrupoSemEmail);
                                    cnpjsTomadorSemEmail.Add(cnpjTomadorSemEmail);
                                }
                                fatuas.Add(codigoFatura);
                            }
                        }
                        else
                        {
                            msgErro += " Não foi encontrado Container disponível para realizar o faturamento";
                        }
                    }
                    else if (tipoAgrupamentoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAgrupamentoFatura.CTe)
                    {
                        List<int> codigos = new List<int>();

                        codigos = repDocumentoFaturamento.ConsultarCTesParaFatura(filtrosTomador);

                        if (codigos != null && codigos.Count > 0)
                        {
                            foreach (var codigo in codigos)
                            {
                                msgErro += "";
                                if (!InserirFatura("", out int codigoGrupoSemEmail, out double cnpjTomadorSemEmail, out int codigoFatura, ref msgErro, 0, 0, "", "", codigo, 0, cnpjTomador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa.Pessoa, observacao, dataInicial, dataFinal, dataFatura, codigoTipoOperacao, codigoOrigem, codigoDestino, numeroBooking, codigoPedidoViagemDirecao, codigoTerminalOrigem, codigoTerminalDestino, tipoPropostaMultimodal, unitOfWork, usuario, stringConexao, tipoServicoMultisoftware, empresa, codigoCarga, codigosCTes, gerarFaturamentoParaClientesExclusivos, tipoCTe))
                                {
                                    codigosGrupoSemEmail.Add(codigoGrupoSemEmail);
                                    cnpjsTomadorSemEmail.Add(cnpjTomadorSemEmail);
                                    return false;
                                }
                                else
                                {
                                    codigosGrupoSemEmail.Add(codigoGrupoSemEmail);
                                    cnpjsTomadorSemEmail.Add(cnpjTomadorSemEmail);
                                }
                                fatuas.Add(codigoFatura);
                            }
                        }
                        else
                        {
                            msgErro += " Não foi encontrado Conhecimento disponível para realizar o faturamento";
                        }
                    }
                    else if (tipoAgrupamentoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAgrupamentoFatura.NavioViagemDirecaoPOL)
                    {
                        List<int> codigos = new List<int>();

                        codigos = repDocumentoFaturamento.ConsultarCodigosPedidoNavioViagemParaFatura(filtrosTomador);

                        if (codigos != null && codigos.Count > 0)
                        {
                            foreach (var codigo in codigos)
                            {
                                msgErro += "";
                                if (!InserirFatura("", out int codigoGrupoSemEmail, out double cnpjTomadorSemEmail, out int codigoFatura, ref msgErro, 0, 0, "", "", 0, 0, cnpjTomador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa.Pessoa, observacao, dataInicial, dataFinal, dataFatura, codigoTipoOperacao, codigoOrigem, codigoDestino, numeroBooking, codigo, codigoTerminalOrigem, codigoTerminalDestino, tipoPropostaMultimodal, unitOfWork, usuario, stringConexao, tipoServicoMultisoftware, empresa, codigoCarga, codigosCTes, gerarFaturamentoParaClientesExclusivos, tipoCTe))
                                {
                                    codigosGrupoSemEmail.Add(codigoGrupoSemEmail);
                                    cnpjsTomadorSemEmail.Add(cnpjTomadorSemEmail);
                                    return false;
                                }
                                else
                                {
                                    codigosGrupoSemEmail.Add(codigoGrupoSemEmail);
                                    cnpjsTomadorSemEmail.Add(cnpjTomadorSemEmail);
                                }
                                fatuas.Add(codigoFatura);
                            }
                        }
                        else
                        {
                            msgErro += " Não foi encontrado Navio/Viagem/Direção disponível para realizar o faturamento";
                        }
                    }
                    else if (tipoAgrupamentoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAgrupamentoFatura.NavioViagemDirecaoPOLPOD)
                    {
                        List<int> codigos = new List<int>();

                        codigos = repDocumentoFaturamento.ConsultarCodigosPedidoNavioViagemParaFatura(filtrosTomador);

                        if (codigos != null && codigos.Count > 0)
                        {
                            foreach (var codigo in codigos)
                            {
                                List<int> codigosTerminalDestino = new List<int>();

                                Dominio.ObjetosDeValor.Embarcador.Fatura.FiltroDocumentosParaFatura filtrosTerminalDestino = new Dominio.ObjetosDeValor.Embarcador.Fatura.FiltroDocumentosParaFatura()
                                {
                                    CPFCNPJTomador = cnpjPessoaPesquisa,
                                    DataInicial = dataInicial,
                                    DataFinal = dataFinal,
                                    TipoOperacao = codigoTipoOperacao,
                                    PedidoViagemNavio = codigo,
                                    TerminalOrigem = codigoTerminalOrigem,
                                    TerminalDestino = codigoTerminalDestino,
                                    Origem = codigoOrigem,
                                    Destino = codigoDestino,
                                    NumeroBooking = numeroBooking,
                                    TipoPropostaMultimodal = tipoPropostaMultimodal,
                                    TiposPropostasMultimodal = tiposPropostasMultimodal,
                                    CodigoCarga = codigoCarga,
                                    ApenasFaturaExclusiva = gerarFaturamentoParaClientesExclusivos,
                                    CodigosCTes = codigosCTes,
                                    TipoCTe = tipoCTe.HasValue ? tipoCTe.Value : Dominio.Enumeradores.TipoCTE.Normal
                                };

                                codigosTerminalDestino = repDocumentoFaturamento.ConsultarCodigosTerminalDestinoParaFatura(filtrosTerminalDestino);

                                if (codigosTerminalDestino != null && codigosTerminalDestino.Count > 0)
                                {
                                    foreach (var codTerminalDestino in codigosTerminalDestino)
                                    {
                                        msgErro += "";
                                        if (!InserirFatura("", out int codigoGrupoSemEmail, out double cnpjTomadorSemEmail, out int codigoFatura, ref msgErro, codigo, 0, "", "", 0, 0, cnpjTomador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa.Pessoa, observacao, dataInicial, dataFinal, dataFatura, codigoTipoOperacao, codigoOrigem, codigoDestino, numeroBooking, codigo, codigoTerminalOrigem, codTerminalDestino, tipoPropostaMultimodal, unitOfWork, usuario, stringConexao, tipoServicoMultisoftware, empresa, codigoCarga, codigosCTes, gerarFaturamentoParaClientesExclusivos, tipoCTe))
                                        {
                                            codigosGrupoSemEmail.Add(codigoGrupoSemEmail);
                                            cnpjsTomadorSemEmail.Add(cnpjTomadorSemEmail);
                                            return false;
                                        }
                                        else
                                        {
                                            codigosGrupoSemEmail.Add(codigoGrupoSemEmail);
                                            cnpjsTomadorSemEmail.Add(cnpjTomadorSemEmail);
                                        }
                                        fatuas.Add(codigoFatura);
                                    }
                                }
                            }
                        }
                        else
                        {
                            msgErro += " Não foi encontrado CT-e disponível para realizar o faturamento, verifique se o mesmo já não foi faturado.";
                        }
                    }
                    else if (tipoAgrupamentoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAgrupamentoFatura.NumeroControleCliente)
                    {
                        List<string> codigos = new List<string>();

                        codigos = repDocumentoFaturamento.ConsultarNumeroControleClienteParaFatura(filtrosTomador);

                        if (codigos != null && codigos.Count > 0 && !string.IsNullOrWhiteSpace(codigos[0]))
                        {
                            foreach (var codigo in codigos)
                            {
                                msgErro += "";
                                if (!InserirFatura("", out int codigoGrupoSemEmail, out double cnpjTomadorSemEmail, out int codigoFatura, ref msgErro, 0, 0, codigo, "", 0, 0, cnpjTomador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa.Pessoa, observacao, dataInicial, dataFinal, dataFatura, codigoTipoOperacao, codigoOrigem, codigoDestino, numeroBooking, codigoPedidoViagemDirecao, codigoTerminalOrigem, codigoTerminalDestino, tipoPropostaMultimodal, unitOfWork, usuario, stringConexao, tipoServicoMultisoftware, empresa, codigoCarga, codigosCTes, gerarFaturamentoParaClientesExclusivos, tipoCTe))
                                {
                                    codigosGrupoSemEmail.Add(codigoGrupoSemEmail);
                                    cnpjsTomadorSemEmail.Add(cnpjTomadorSemEmail);
                                    return false;
                                }
                                else
                                {
                                    codigosGrupoSemEmail.Add(codigoGrupoSemEmail);
                                    cnpjsTomadorSemEmail.Add(cnpjTomadorSemEmail);
                                }
                                fatuas.Add(codigoFatura);
                            }
                        }
                        else
                        {
                            msgErro += " Não foi encontrado Número de Controle disponível para realizar o faturamento";
                        }
                    }
                    else if (tipoAgrupamentoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAgrupamentoFatura.NumeroReferenciaEDI)
                    {
                        List<string> codigos = new List<string>();

                        codigos = repDocumentoFaturamento.ConsultarNumeroReferenciaEDIParaFatura(filtrosTomador);

                        if (codigos != null && codigos.Count > 0 && !string.IsNullOrWhiteSpace(codigos[0]))
                        {
                            foreach (var codigo in codigos)
                            {
                                msgErro += "";
                                if (!InserirFatura("", out int codigoGrupoSemEmail, out double cnpjTomadorSemEmail, out int codigoFatura, ref msgErro, 0, 0, "", codigo, 0, 0, cnpjTomador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa.Pessoa, observacao, dataInicial, dataFinal, dataFatura, codigoTipoOperacao, codigoOrigem, codigoDestino, numeroBooking, codigoPedidoViagemDirecao, codigoTerminalOrigem, codigoTerminalDestino, tipoPropostaMultimodal, unitOfWork, usuario, stringConexao, tipoServicoMultisoftware, empresa, codigoCarga, codigosCTes, gerarFaturamentoParaClientesExclusivos, tipoCTe))
                                {
                                    codigosGrupoSemEmail.Add(codigoGrupoSemEmail);
                                    cnpjsTomadorSemEmail.Add(cnpjTomadorSemEmail);
                                    return false;
                                }
                                else
                                {
                                    codigosGrupoSemEmail.Add(codigoGrupoSemEmail);
                                    cnpjsTomadorSemEmail.Add(cnpjTomadorSemEmail);
                                }
                                fatuas.Add(codigoFatura);
                            }
                        }
                        else
                        {
                            msgErro += " Não foi encontrado Número de Referencia de EDI disponível para realizar o faturamento";
                        }
                    }

                    unitOfWork.FlushAndClear();
                }
            }
            else
            {
                msgErro = "Não foi encontrado nenhuma Pessoa com pendência de faturamento";
                return false;
            }

            if (codigosGrupoSemEmail.Count > 0 || cnpjsTomadorSemEmail.Count > 0)
                GerarNotificacaoTomadorSemEmail(codigosGrupoSemEmail, cnpjsTomadorSemEmail, codigoPedidoViagemDirecao, codigoTerminalOrigem, unitOfWork);

            return true;
        }

        private static bool InserirFatura(string ieTomador, out int codigoGrupoSemEmail, out double cnpjTomadorSemEmail, out int codigoFatura, ref string msgErro, int codigoMDFe, int codigoContainer, string numeroControleCliente, string numeroReferenciaEDI, int codigoCTe, int codigoGrupo, double cnpjTomador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa tipoPessoa, string observacao, DateTime? dataInicial, DateTime? dataFinal, DateTime dataFatura, int codigoTipoOperacao, int codigoOrigem, int codigoDestino, string numeroBooking, int codigoPedidoViagemDirecao, int codigoTerminalOrigem, int codigoTerminalDestino, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal> tipoPropostaMultimodal, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Usuario usuario, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Empresa empresa, int codigoCarga, List<int> codigosCTes, bool faturamentoExclusivo, Dominio.Enumeradores.TipoCTE? tipoCTe)
        {
            msgErro = "";
            codigoFatura = 0;
            codigoGrupoSemEmail = 0;
            cnpjTomadorSemEmail = 0;

            Servicos.Embarcador.Fatura.Fatura servFatura = new Servicos.Embarcador.Fatura.Fatura(unitOfWork);
            Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(stringConexao, null, tipoServicoMultisoftware, string.Empty);

            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);
            Repositorio.Embarcador.Pedidos.Container repContainer = new Repositorio.Embarcador.Pedidos.Container(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.Embarcador.Fatura.FaturaCarga repFaturaCarga = new Repositorio.Embarcador.Fatura.FaturaCarga(unitOfWork);
            Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);
            Repositorio.Embarcador.Fatura.FaturaLoteCTe repFaturaLoteCTe = new Repositorio.Embarcador.Fatura.FaturaLoteCTe(unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoViagemNavio repPedidoViagemNavio = new Repositorio.Embarcador.Pedidos.PedidoViagemNavio(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoTerminalImportacao repTerminal = new Repositorio.Embarcador.Pedidos.TipoTerminalImportacao(unitOfWork);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Configuracoes.AcordoFaturamentoCliente repAcordoFaturamento = new Repositorio.Embarcador.Configuracoes.AcordoFaturamentoCliente(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo(0);

            unitOfWork.Start();

            //if(!faturamentoExclusivo && codigoCarga == 0)
            //    return false;

            Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoCliente acordoFaturamento = null;
            if (codigoGrupo > 0)
                acordoFaturamento = repAcordoFaturamento.BuscarAcordoCliente(0, codigoGrupo);
            if (cnpjTomador > 0)
                acordoFaturamento = repAcordoFaturamento.BuscarAcordoCliente(cnpjTomador, 0);

            if (!faturamentoExclusivo && acordoFaturamento != null && acordoFaturamento.FaturamentoPermissaoExclusiva && codigoCarga == 0)
            {
                msgErro = ("Não foi possível gerar o fautramento pois o acordo do cliente está confiurado para ser de forma exclusiva");
                return false;
            }

            Dominio.Entidades.Embarcador.Fatura.Fatura fatura = new Dominio.Entidades.Embarcador.Fatura.Fatura();

            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado();
            auditado.TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Usuario;
            auditado.OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema;
            auditado.Usuario = usuario;
            auditado.Empresa = empresa;

            if (dataInicial.HasValue && dataInicial.Value > DateTime.MinValue)
                fatura.DataInicial = dataInicial;
            else
                fatura.DataInicial = null;
            if (dataFinal.HasValue && dataFinal.Value > DateTime.MinValue)
                fatura.DataFinal = dataFinal;
            else
                fatura.DataFinal = null;
            fatura.DataFatura = DateTime.Now.Date;
            fatura.NovoModelo = true;
            fatura.Etapa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFatura.LancandoCargas;
            fatura.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.EmAntamento;
            fatura.Observacao = observacao;
            fatura.Usuario = usuario;
            fatura.TipoPessoa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa.GrupoPessoa;
            fatura.GerarDocumentosAutomaticamente = true;

            fatura.AliquotaICMS = null;
            fatura.TipoCarga = null;
            fatura.Cliente = cnpjTomador > 0 ? repCliente.BuscarPorCPFCNPJ(cnpjTomador) : null;
            fatura.IETomador = ieTomador;
            fatura.GrupoPessoas = codigoGrupo > 0 ? repGrupoPessoas.BuscarPorCodigo(codigoGrupo) : null;
            fatura.TipoOperacao = codigoTipoOperacao > 0 ? repTipoOperacao.BuscarPorCodigo(codigoTipoOperacao) : null;
            fatura.Transportador = null;
            fatura.MDFe = codigoMDFe > 0 ? repMDFe.BuscarPorCodigo(codigoMDFe) : null;
            fatura.Container = codigoContainer > 0 ? repContainer.BuscarPorCodigo(codigoContainer) : null;
            fatura.NumeroControleCliente = numeroControleCliente;
            fatura.NumeroReferenciaEDI = numeroReferenciaEDI;
            fatura.CTe = codigoCTe > 0 ? repCTe.BuscarPorCodigo(codigoCTe) : null;
            fatura.Carga = codigoCarga > 0 ? repCarga.BuscarPorCodigo(codigoCarga) : null;

            fatura.PedidoViagemNavio = codigoPedidoViagemDirecao > 0 ? repPedidoViagemNavio.BuscarPorCodigo(codigoPedidoViagemDirecao) : null;
            fatura.TerminalOrigem = codigoTerminalOrigem > 0 ? repTerminal.BuscarPorCodigo(codigoTerminalOrigem) : null;
            fatura.TerminalDestino = codigoTerminalDestino > 0 ? repTerminal.BuscarPorCodigo(codigoTerminalDestino) : null;
            fatura.Origem = codigoOrigem > 0 ? repLocalidade.BuscarPorCodigo(codigoOrigem) : null;
            fatura.Destino = codigoDestino > 0 ? repLocalidade.BuscarPorCodigo(codigoDestino) : null;
            fatura.NumeroBooking = numeroBooking;
            fatura.TipoPropostaMultimodal = tipoPropostaMultimodal;
            fatura.GeradoPorFaturaLote = true;

            fatura.ImprimeObservacaoFatura = false;
            fatura.Total = 0;
            fatura.Numero = 0;
            fatura.DataFatura = DateTime.Now.Date;
            fatura.DataCancelamentoFatura = null;
            fatura.FaturamentoExclusivo = faturamentoExclusivo;
            fatura.TipoCTe = tipoCTe;

            if (fatura.Carga != null && fatura.Carga.CargaTakeOrPay)
            {
                fatura.ImprimeObservacaoFatura = true;
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPrimeiraPorCarga(fatura.Carga.Codigo);

                if (cargaPedido?.TipoPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.DemurrageCabotagem || cargaPedido?.TipoPropostaMultimodal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.DetentionCabotagem)
                {
                    fatura.ObservacaoFatura += "Navio/Viagem/Direção: " + (fatura.Carga.PedidoViagemNavio?.Descricao ?? "") + " \n";
                    fatura.ObservacaoFatura += "Porto de Origem: " + (fatura.Carga.PortoOrigem?.Descricao ?? "") + "\n";
                    fatura.ObservacaoFatura += "Porto de Destino:" + (fatura.Carga.PortoDestino?.Descricao ?? "") + "\n";
                    fatura.ObservacaoFatura += "Tipo Proposta: " + (cargaPedido?.TipoPropostaMultimodal.ObterDescricao()) + "\n" + "\n";
                }
                else if (cargaPedido?.TipoPropostaMultimodal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.FaturamentoContabilidade)
                {
                    int qtdDisponibilizadas = repCargaPedido.BuscarQuantidadeDisponibilizadas(fatura.Carga.Codigo);
                    int qtdNaoEmbarcadas = repCargaPedido.BuscarQuantidadeNaoEmbarcadas(fatura.Carga.Codigo);

                    fatura.ObservacaoFatura = "Fatura de Penalidade Contratual \n\n";
                    fatura.ObservacaoFatura += "Navio/Viagem/Direção: " + (fatura.Carga.PedidoViagemNavio?.Descricao ?? "") + " \n";
                    fatura.ObservacaoFatura += "Porto de Origem: " + (fatura.Carga.PortoOrigem?.Descricao ?? "") + "\n";
                    fatura.ObservacaoFatura += "Porto de Destino: " + (fatura.Carga.PortoDestino?.Descricao ?? "") + "\n";
                    fatura.ObservacaoFatura += "Quantidade de unidades disponibilizadas: " + (Utilidades.String.OnlyNumbers(qtdDisponibilizadas.ToString("n0"))) + " \n";
                    fatura.ObservacaoFatura += "Quantidade de unidades não embarcadas: " + (Utilidades.String.OnlyNumbers(qtdNaoEmbarcadas.ToString("n0"))) + "\n";
                }
                else
                    fatura.FaturaPropostaFaturamento = true;

                if (!string.IsNullOrWhiteSpace(fatura.Carga.ObservacaoParaFaturamento))
                {
                    if (!string.IsNullOrWhiteSpace(fatura.ObservacaoFatura))
                        fatura.ObservacaoFatura += "\n\n";
                    fatura.ObservacaoFatura += fatura.Carga.ObservacaoParaFaturamento;
                }
            }

            if (configuracaoTMS.GerarNumeracaoFaturaAnual)
            {
                int anoAtual = DateTime.Now.Year;
                fatura.ControleNumeracao = repFatura.BuscarProximoControleNumeracao(anoAtual);
                anoAtual = (anoAtual % 100);
                if (fatura.ControleNumeracao == 1 || (fatura.ControleNumeracao < ((anoAtual * 1000000) + 1)))
                    fatura.ControleNumeracao = (anoAtual * 1000000) + 1;
            }
            else
                fatura.ControleNumeracao = repFatura.BuscarProximoControleNumeracao();
            List<string> emails;
            if (TomadorPossuiEmailConfigurado(fatura, unitOfWork, out emails))
            {
                repFatura.Inserir(fatura, auditado);

                if (codigosCTes != null && codigosCTes.Count > 0)
                {
                    foreach (var codigoCTeLote in codigosCTes)
                    {
                        Dominio.Entidades.Embarcador.Fatura.FaturaLoteCTe faturaLoteCTe = new Dominio.Entidades.Embarcador.Fatura.FaturaLoteCTe()
                        {
                            CTe = repCTe.BuscarPorCodigo(codigoCTeLote),
                            Fatura = fatura
                        };
                        repFaturaLoteCTe.Inserir(faturaLoteCTe);
                    }
                }

                codigoFatura = fatura.Codigo;

                Servicos.Auditoria.Auditoria.Auditar(auditado, fatura, null, "Gerou a Fatura por lote.", unitOfWork);

                if (usuario != null)
                    servFatura.InserirLog(fatura, unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogFatura.IniciouFatura, usuario);

                unitOfWork.CommitChanges();
            }
            else
            {
                unitOfWork.Rollback();

                try
                {
                    if (usuario != null)
                        serNotificacao.GerarNotificacao(usuario, 0, "Faturas/FaturaLote", string.Format(Localization.Resources.Configuracao.Fatura.NaoFoiPossivelEncontrarEmailConfiguradoGerarFaturaTomador, (fatura.Cliente?.Descricao ?? fatura.GrupoPessoas?.Descricao ?? "")), IconesNotificacao.atencao, TipoNotificacao.todas, tipoServicoMultisoftware, unitOfWork);

                    codigoGrupoSemEmail = fatura.GrupoPessoas?.Codigo ?? 0;
                    cnpjTomadorSemEmail = fatura.Cliente?.CPF_CNPJ ?? 0;

                    msgErro = ("Não foi possível encontrar nenhum e-mail configurado para gerar a fatura do tomador " + (fatura.Cliente?.Descricao ?? fatura.GrupoPessoas?.Descricao ?? ""));
                    return true;
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex, "FaturamentoLote");
                }
            }

            return true;
        }

        private static bool TomadorPossuiEmailConfigurado(Dominio.Entidades.Embarcador.Fatura.Fatura fatura, Repositorio.UnitOfWork unitOfWork, out List<string> emails)
        {
            emails = new List<string>();

            Repositorio.Embarcador.Configuracoes.AcordoFaturamentoCliente repAcordo = new Repositorio.Embarcador.Configuracoes.AcordoFaturamentoCliente(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Configuracoes.AcordoFaturamentoEmailCabotagem repEmailCabotagem = new Repositorio.Embarcador.Configuracoes.AcordoFaturamentoEmailCabotagem(unitOfWork);
            Repositorio.Embarcador.Configuracoes.AcordoFaturamentoEmailLongoCurso repEmailLongoCurso = new Repositorio.Embarcador.Configuracoes.AcordoFaturamentoEmailLongoCurso(unitOfWork);
            Repositorio.Embarcador.Configuracoes.AcordoFaturamentoEmailCustoExtra repEmailCustoExtra = new Repositorio.Embarcador.Configuracoes.AcordoFaturamentoEmailCustoExtra(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            if (!configuracaoTMS.AtivarFaturamentoAutomatico)
                return true;

            if (fatura.Carga?.CargaTakeOrPay ?? false)
                return true;

            Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoCliente acordoCliente = null;
            if (fatura.Cliente != null)
            {
                acordoCliente = repAcordo.BuscarAcordoCliente(fatura.Cliente.CPF_CNPJ, 0);
                if (acordoCliente == null && fatura.Cliente.GrupoPessoas != null)
                    acordoCliente = repAcordo.BuscarAcordoCliente(0, fatura.Cliente.GrupoPessoas.Codigo);
            }
            if (acordoCliente == null && fatura.GrupoPessoas != null)
                acordoCliente = repAcordo.BuscarAcordoCliente(0, fatura.GrupoPessoas.Codigo);

            Dominio.Entidades.Cliente pessoaAcordoFaturamento = null;
            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoasAcordoFaturamento = null;
            pessoaAcordoFaturamento = fatura.Cliente;
            grupoPessoasAcordoFaturamento = fatura.GrupoPessoas;
            if (pessoaAcordoFaturamento == null)
                pessoaAcordoFaturamento = fatura.ClienteTomadorFatura;
            if (grupoPessoasAcordoFaturamento == null)
                grupoPessoasAcordoFaturamento = fatura.Cliente?.GrupoPessoas;
            if (grupoPessoasAcordoFaturamento == null)
                grupoPessoasAcordoFaturamento = fatura.ClienteTomadorFatura?.GrupoPessoas;

            if (acordoCliente != null)
            {
                if (pessoaAcordoFaturamento != null || grupoPessoasAcordoFaturamento != null)
                {
                    List<Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoEmailCabotagem> emailsCabotagem = null;
                    if (pessoaAcordoFaturamento != null)
                        emailsCabotagem = repEmailCabotagem.BuscarPorPessoa(pessoaAcordoFaturamento.CPF_CNPJ);

                    if (grupoPessoasAcordoFaturamento != null && emailsCabotagem == null)
                        emailsCabotagem = repEmailCabotagem.BuscarPorGrupoPessoa(grupoPessoasAcordoFaturamento.Codigo);

                    if (emailsCabotagem != null && emailsCabotagem.Count > 0)
                    {
                        foreach (var emailCabotagem in emailsCabotagem)
                        {
                            if (!string.IsNullOrWhiteSpace(emailCabotagem.Email))
                                emails.AddRange(emailCabotagem.Email.Split(';').ToList());
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(acordoCliente.CabotagemEmail))
                        emails.AddRange(acordoCliente.CabotagemEmail.Split(';').ToList());
                }
            }

            Dominio.Entidades.Cliente tomador = fatura.Cliente;

            if (!configuracaoTMS.UtilizaEmissaoMultimodal)
            {
                if (!string.IsNullOrWhiteSpace(fatura?.ClienteTomadorFatura?.EmailFatura))
                    emails.AddRange(fatura.ClienteTomadorFatura.EmailFatura.Split(';').ToList());
                else if (!string.IsNullOrWhiteSpace(fatura?.ClienteTomadorFatura?.GrupoPessoas?.EmailFatura))
                    emails.AddRange(fatura.ClienteTomadorFatura.GrupoPessoas.EmailFatura.Split(';').ToList());
            }

            if (tomador != null)
            {
                if (!configuracaoTMS.UtilizaEmissaoMultimodal)
                {
                    if (!string.IsNullOrWhiteSpace(tomador.EmailFatura))
                        emails.AddRange(tomador.EmailFatura.Split(';').ToList());
                    else if (!string.IsNullOrWhiteSpace(tomador.GrupoPessoas?.EmailFatura))
                        emails.AddRange(tomador.GrupoPessoas.EmailFatura.Split(';').ToList());

                    if (!string.IsNullOrWhiteSpace(tomador.Email))
                        emails.AddRange(tomador.Email.Split(';').ToList());

                    for (int a = 0; a < tomador.Emails.Count; a++)
                    {
                        Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEmail outroEmail = tomador.Emails[a];
                        if (!string.IsNullOrWhiteSpace(outroEmail.Email) && outroEmail.EmailStatus == "A"
                            && outroEmail.TipoEmail != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmail.Administrativo)
                            emails.Add(outroEmail.Email);
                    }
                }
            }
            else
            {
                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = fatura.GrupoPessoas;

                if (!configuracaoTMS.UtilizaEmissaoMultimodal)
                {
                    if (!string.IsNullOrWhiteSpace(grupoPessoas.EmailFatura))
                        emails.AddRange(grupoPessoas.EmailFatura.Split(';').ToList());

                    if (!string.IsNullOrWhiteSpace(grupoPessoas.Email))
                        emails.AddRange(grupoPessoas.Email.Split(';').ToList());
                }
            }

            emails = emails.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();

            if (emails == null || emails.Count == 0)
                return false;
            else
                return true;
        }

        private static bool GerarNotificacaoConclusaoFaturamento(List<int> faturas, long codigoFaturamento, Repositorio.UnitOfWork unitOfWork, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(stringConexao, null, tipoServicoMultisoftware, string.Empty);
            Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork);
            Repositorio.Embarcador.Fatura.FaturamentoLote repFaturamentoLote = new Repositorio.Embarcador.Fatura.FaturamentoLote(unitOfWork);
            Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(unitOfWork);

            Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo(0);
            Dominio.Entidades.Embarcador.Fatura.FaturamentoLote faturamentoLote = repFaturamentoLote.BuscarPorCodigo(codigoFaturamento);
            List<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento> documentos = repFaturaDocumento.BuscarPorFatura(faturas);

            if (faturamentoLote.Usuario != null)
                serNotificacao.GerarNotificacao(faturamentoLote.Usuario, 0, "Faturas/FaturaLote", Localization.Resources.Configuracao.Fatura.ConclusaoFaturamentoLoteVerifiqueEmail, IconesNotificacao.sucesso, TipoNotificacao.todas, tipoServicoMultisoftware, unitOfWork);

            string assunto = "Conclusão do Faturamento em Lote - " + (faturamentoLote?.PedidoViagemNavio?.Descricao ?? "") + " - " + (faturamentoLote?.TerminalOrigem?.Porto?.Descricao ?? "");

            string mensagemEmail = "<br/>Segue CT-e faturados pelo procedimento em lote:<br/>";

            mensagemEmail += "<script src='https://ajax.googleapis.com/ajax/libs/jquery/1.9.1/jquery.min.js'></script>";
            mensagemEmail += "<script src='http://www.developerdan.com/table-to-json/javascripts/jquery.tabletojson.min.js'></script>";
            mensagemEmail += "<table style='width:100%; align='center'; border='1'; cellpadding='0'; cellspacing='0';>";
            mensagemEmail += "<thead>";
            mensagemEmail += "<th>Tomador</th>";
            mensagemEmail += "<th>CNPJ Tomador</th>";
            mensagemEmail += "<th>Grupo</th>";
            mensagemEmail += "<th>Nº CTe</th>";
            mensagemEmail += "<th>Nº Controle</th>";
            mensagemEmail += "<th>Viagem</th>";
            mensagemEmail += "<th>Porto de Origem</th>";
            mensagemEmail += "<th>Booking</th>";
            mensagemEmail += "<th>Container</th>";
            mensagemEmail += "</thead>";
            mensagemEmail += "<tbody>";

            foreach (var documento in documentos)
            {
                mensagemEmail += "<tr>";
                mensagemEmail += "<td>" + (documento.Documento.CTe?.TomadorPagador?.Nome ?? "") + "</td>";
                mensagemEmail += "<td>" + (documento.Documento.CTe?.TomadorPagador?.CPF_CNPJ ?? "") + "</td>";
                mensagemEmail += "<td>" + (documento.Documento.CTe?.TomadorPagador?.GrupoPessoas?.Descricao ?? "") + "</td>";
                mensagemEmail += "<td>" + Utilidades.String.OnlyNumbers((documento.Documento.CTe?.Numero.ToString("n0") ?? "")) + "</td>";
                mensagemEmail += "<td>" + (documento.Documento.CTe?.NumeroControle ?? "") + "</td>";
                mensagemEmail += "<td>" + (documento.Documento.CTe?.Viagem?.Descricao ?? "") + "</td>";
                mensagemEmail += "<td>" + (documento.Documento.CTe?.PortoOrigem?.Descricao ?? "") + "</td>";
                mensagemEmail += "<td>" + (documento.Documento.CTe?.NumeroBooking ?? "") + "</td>";
                mensagemEmail += "<td>" + (documento.Documento.CTe?.ListaContainer ?? "") + "</td>";
                mensagemEmail += "</tr>";
            }
            mensagemEmail += "</tbody></table>";
            mensagemEmail += "<br/><br/>E -mail enviado automaticamente. Por favor, não responda.";

            List<string> emailsEnvio = new List<string>();
            if (faturamentoLote.Usuario != null && !string.IsNullOrWhiteSpace(faturamentoLote.Usuario.Email))
                emailsEnvio.AddRange(faturamentoLote.Usuario.Email.Split(';').ToList());
            if (faturamentoLote.Empresa != null && !string.IsNullOrWhiteSpace(faturamentoLote.Empresa.Email))
                emailsEnvio.AddRange(faturamentoLote.Empresa.Email.Split(';').ToList());
            if (faturamentoLote.Empresa != null && !string.IsNullOrWhiteSpace(faturamentoLote.Empresa.EmailAdministrativo))
                emailsEnvio.AddRange(faturamentoLote.Empresa.EmailAdministrativo.Split(';').ToList());

            if (emailsEnvio != null && emailsEnvio.Count > 0 && email != null)
                Servicos.Email.EnviarEmail(email.Email, email.Email, email.Senha, null, emailsEnvio.ToArray(), null, assunto, mensagemEmail, email.Smtp, out string mensagemErro, email.DisplayEmail, null, "", email.RequerAutenticacaoSmtp, "", email.PortaSmtp, unitOfWork);

            return true;
        }

        private static bool GerarNotificacaoConclusaoCancelamento(List<int> faturas, long codigoFaturamento, Repositorio.UnitOfWork unitOfWork, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(stringConexao, null, tipoServicoMultisoftware, string.Empty);
            Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork);
            Repositorio.Embarcador.Fatura.FaturamentoLote repFaturamentoLote = new Repositorio.Embarcador.Fatura.FaturamentoLote(unitOfWork);
            Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(unitOfWork);
            Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);

            Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo(0);
            Dominio.Entidades.Embarcador.Fatura.FaturamentoLote faturamentoLote = repFaturamentoLote.BuscarPorCodigo(codigoFaturamento);
            List<Dominio.Entidades.Embarcador.Fatura.FaturaDocumento> documentos = repFaturaDocumento.BuscarPorFatura(faturas);
            Dominio.Entidades.Embarcador.Fatura.Fatura fatura = documentos?.FirstOrDefault()?.Fatura ?? null;
            if (fatura == null && faturas != null && faturas.Count > 0)
                fatura = repFatura.BuscarPorCodigo(faturas.FirstOrDefault());

            if (faturamentoLote == null && fatura != null && fatura.UsuarioCancelamento != null)
                serNotificacao.GerarNotificacao(fatura.UsuarioCancelamento, 0, "Faturas/Fatura", Localization.Resources.Configuracao.Fatura.ConclusaoCancelamentoFaturaVerifiqueEmail, IconesNotificacao.sucesso, TipoNotificacao.todas, tipoServicoMultisoftware, unitOfWork);
            else if (faturamentoLote.Usuario != null)
                serNotificacao.GerarNotificacao(faturamentoLote.Usuario, 0, "Faturas/FaturaCancelamentoLote", Localization.Resources.Configuracao.Fatura.ConclusaoCancelamentoFaturaLoteVerifiqueEmail, IconesNotificacao.sucesso, TipoNotificacao.todas, tipoServicoMultisoftware, unitOfWork);

            string assunto = "Conclusão do Cancelamento da Fatura - " + (faturamentoLote?.PedidoViagemNavio?.Descricao ?? fatura?.PedidoViagemNavio?.Descricao ?? Utilidades.String.OnlyNumbers(fatura?.Numero.ToString("n0") ?? "")) + " - " + (faturamentoLote?.TerminalOrigem?.Porto?.Descricao ?? fatura?.TerminalOrigem?.Porto?.Descricao ?? "");

            string mensagemEmail = "<br/>Segue CT-e com a fatura cancelada:<br/>";

            if (documentos != null && documentos.Count > 0)
            {
                mensagemEmail += "<script src='https://ajax.googleapis.com/ajax/libs/jquery/1.9.1/jquery.min.js'></script>";
                mensagemEmail += "<script src='http://www.developerdan.com/table-to-json/javascripts/jquery.tabletojson.min.js'></script>";
                mensagemEmail += "<table style='width:100%; align='center'; border='1'; cellpadding='0'; cellspacing='0';>";
                mensagemEmail += "<thead>";
                mensagemEmail += "<th>Nº Fatura</th>";
                mensagemEmail += "<th>Tomador</th>";
                mensagemEmail += "<th>CNPJ Tomador</th>";
                mensagemEmail += "<th>Grupo</th>";
                mensagemEmail += "<th>Nº CTe</th>";
                mensagemEmail += "<th>Nº Controle</th>";
                mensagemEmail += "<th>Viagem</th>";
                mensagemEmail += "<th>Porto de Origem</th>";
                mensagemEmail += "<th>Booking</th>";
                mensagemEmail += "<th>Container</th>";
                mensagemEmail += "</thead>";
                mensagemEmail += "<tbody>";


                foreach (var documento in documentos)
                {
                    mensagemEmail += "<tr>";
                    mensagemEmail += "<td>" + Utilidades.String.OnlyNumbers((documento.Fatura?.Numero.ToString("n0") ?? "")) + "</td>";
                    mensagemEmail += "<td>" + (documento.Documento.CTe?.TomadorPagador?.Nome ?? "") + "</td>";
                    mensagemEmail += "<td>" + (documento.Documento.CTe?.TomadorPagador?.CPF_CNPJ ?? "") + "</td>";
                    mensagemEmail += "<td>" + (documento.Documento.CTe?.TomadorPagador?.GrupoPessoas?.Descricao ?? "") + "</td>";
                    mensagemEmail += "<td>" + Utilidades.String.OnlyNumbers((documento.Documento.CTe?.Numero.ToString("n0") ?? "")) + "</td>";
                    mensagemEmail += "<td>" + (documento.Documento.CTe?.NumeroControle ?? "") + "</td>";
                    mensagemEmail += "<td>" + (documento.Documento.CTe?.Viagem?.Descricao ?? "") + "</td>";
                    mensagemEmail += "<td>" + (documento.Documento.CTe?.PortoOrigem?.Descricao ?? "") + "</td>";
                    mensagemEmail += "<td>" + (documento.Documento.CTe?.NumeroBooking ?? "") + "</td>";
                    mensagemEmail += "<td>" + (documento.Documento.CTe?.ListaContainer ?? "") + "</td>";
                    mensagemEmail += "</tr>";
                }
                mensagemEmail += "</tbody></table>";
            }

            mensagemEmail += "<br/><br/>E -mail enviado automaticamente. Por favor, não responda.";

            List<string> emails = null;
            if (documentos != null && documentos.Count > 0 && documentos.FirstOrDefault().Fatura != null)
                TomadorPossuiEmailConfigurado(documentos.FirstOrDefault().Fatura, unitOfWork, out emails);
            else if (fatura != null)
                TomadorPossuiEmailConfigurado(fatura, unitOfWork, out emails);

            List<string> emailsEnvio = new List<string>();
            if (fatura != null && faturamentoLote == null)
            {
                if (fatura.UsuarioCancelamento != null && !string.IsNullOrWhiteSpace(fatura.UsuarioCancelamento.Email))
                    emailsEnvio.AddRange(fatura.UsuarioCancelamento.Email.Split(';').ToList());
                if (!string.IsNullOrWhiteSpace(fatura?.TerminalDestino?.Porto?.Empresa?.Email ?? ""))
                    emailsEnvio.AddRange(fatura?.TerminalDestino?.Porto?.Empresa?.Email.Split(';').ToList());
                if (!string.IsNullOrWhiteSpace(fatura?.TerminalDestino?.Porto?.Empresa?.EmailAdministrativo ?? ""))
                    emailsEnvio.AddRange(fatura?.TerminalDestino?.Porto?.Empresa?.EmailAdministrativo.Split(';').ToList());
                if (!string.IsNullOrWhiteSpace(fatura?.TerminalOrigem?.Porto?.Empresa?.Email ?? ""))
                    emailsEnvio.AddRange(fatura?.TerminalOrigem?.Porto?.Empresa?.Email.Split(';').ToList());
                if (!string.IsNullOrWhiteSpace(fatura?.TerminalOrigem?.Porto?.Empresa?.EmailAdministrativo ?? ""))
                    emailsEnvio.AddRange(fatura?.TerminalOrigem?.Porto?.Empresa?.EmailAdministrativo.Split(';').ToList());

                List<string> emailsTomador = Servicos.Embarcador.Fatura.Fatura.BuscarEmailAcordoFaturamentoTomadorFatura(faturas, unitOfWork);
                if (emailsTomador != null && emailsTomador.Count > 0)
                    emailsEnvio.AddRange(emailsTomador);
            }
            else
            {
                if (faturamentoLote.Usuario != null && !string.IsNullOrWhiteSpace(faturamentoLote.Usuario.Email))
                    emailsEnvio.AddRange(faturamentoLote.Usuario.Email.Split(';').ToList());
                if (faturamentoLote.Empresa != null && !string.IsNullOrWhiteSpace(faturamentoLote.Empresa.Email))
                    emailsEnvio.AddRange(faturamentoLote.Empresa.Email.Split(';').ToList());
                if (faturamentoLote.Empresa != null && !string.IsNullOrWhiteSpace(faturamentoLote.Empresa.EmailAdministrativo))
                    emailsEnvio.AddRange(faturamentoLote.Empresa.EmailAdministrativo.Split(';').ToList());
            }

            if (emails != null && emails.Count > 0)
                emailsEnvio.AddRange(emails);

            if (emailsEnvio != null && emailsEnvio.Count > 0 && email != null)
                Servicos.Email.EnviarEmail(email.Email, email.Email, email.Senha, null, emailsEnvio.ToArray(), null, assunto, mensagemEmail, email.Smtp, out string mensagemErro, email.DisplayEmail, null, "", email.RequerAutenticacaoSmtp, "", email.PortaSmtp, unitOfWork);

            return true;
        }

        private static void GerarNotificacaoTomadorSemEmail(List<int> codigosGrupoSemEmail, List<double> cnpjsTomadorSemEmail, int codigoPedidoViagemDirecao, int codigoTerminalOrigem, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                Repositorio.Embarcador.Pedidos.PedidoViagemNavio repViagem = new Repositorio.Embarcador.Pedidos.PedidoViagemNavio(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoTerminalImportacao repTerminal = new Repositorio.Embarcador.Pedidos.TipoTerminalImportacao(unitOfWork);
                Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork);
                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

                Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo(0);

                Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao terminal = null;
                Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio viagem = null;
                if (codigoTerminalOrigem > 0)
                    terminal = repTerminal.BuscarPorCodigo(codigoTerminalOrigem);
                if (codigoPedidoViagemDirecao > 0)
                    viagem = repViagem.BuscarPorCodigo(codigoPedidoViagemDirecao);

                Dominio.Entidades.Empresa empresa = terminal?.Empresa;

                if (codigosGrupoSemEmail != null && codigosGrupoSemEmail.Count > 0)
                    codigosGrupoSemEmail = codigosGrupoSemEmail.Distinct().ToList();
                if (cnpjsTomadorSemEmail != null && cnpjsTomadorSemEmail.Count > 0)
                    cnpjsTomadorSemEmail = cnpjsTomadorSemEmail.Distinct().ToList();

                string assunto = "Tomador sem e-mail configurado para faturamento - " + (viagem?.Descricao ?? "") + " - " + (terminal?.Porto?.Descricao ?? "");

                string corpoEmail = ("<br/>Não foi possível encontrar nenhum e-mail configurado para gerar a fatura aos tomadores:<br/><br/> ");

                if (codigosGrupoSemEmail != null && codigosGrupoSemEmail.Count > 0)
                {
                    foreach (var codigoGrupo in codigosGrupoSemEmail)
                    {
                        if (codigoGrupo > 0)
                            corpoEmail += "Grupo Tomador: " + (repGrupoPessoas.BuscarPorCodigo(codigoGrupo)?.Descricao ?? "") + " <br/>";
                    }
                }
                if (cnpjsTomadorSemEmail != null && cnpjsTomadorSemEmail.Count > 0)
                {
                    foreach (var cnpj in cnpjsTomadorSemEmail)
                    {
                        if (cnpj > 0)
                            corpoEmail += "Tomador: " + (repCliente.BuscarPorCPFCNPJ(cnpj)?.Descricao ?? "") + " <br/>";
                    }
                }

                List<string> emailsEnvio = new List<string>();
                if (empresa != null && !string.IsNullOrWhiteSpace(empresa.Email))
                    emailsEnvio.AddRange(empresa.Email.Split(';').ToList());
                if (empresa != null && !string.IsNullOrWhiteSpace(empresa.EmailAdministrativo))
                    emailsEnvio.AddRange(empresa.EmailAdministrativo.Split(';').ToList());

                if (emailsEnvio != null && emailsEnvio.Count > 0 && email != null)
                    Servicos.Email.EnviarEmail(email.Email, email.Email, email.Senha, null, emailsEnvio.ToArray(), null, assunto, corpoEmail, email.Smtp, out string mensagemErro, email.DisplayEmail, null, "", email.RequerAutenticacaoSmtp, "", email.PortaSmtp, unitOfWork);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "FaturamentoLote");
            }
        }


    }
}
