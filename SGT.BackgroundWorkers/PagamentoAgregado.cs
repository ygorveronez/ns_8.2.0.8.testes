using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdminMultisoftware.Repositorio;
using SGT.BackgroundWorkers.Utils;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 10000)]

    public class PagamentoAgregado : LongRunningProcessBase<PagamentoAgregado>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            ProcessarDocumentosPagamentoAgregado(unitOfWork, unitOfWorkAdmin, _codigoEmpresa, _stringConexao, _tipoServicoMultisoftware, _stringConexaoAdmin, _clienteMultisoftware.Codigo);
            ProcessarDocumentosManuaisPagamentoAgregado(unitOfWork, unitOfWorkAdmin, _codigoEmpresa, _stringConexao, _tipoServicoMultisoftware, _stringConexaoAdmin, _clienteMultisoftware.Codigo);
        }

        public override bool CanRun()
        {
            return _tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS;
        }

        private void ProcessarDocumentosPagamentoAgregado(Repositorio.UnitOfWork unitOfWork, UnitOfWork unitOfWorkAdmin, int codigoEmpresa, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string stringConexaoAdmin, int clienteCodigo)
        {
            Dominio.Entidades.Usuario usuarioPagamento = null;
            int codigoPagamento = 0;

            AdminMultisoftware.Repositorio.Pessoas.Cliente repCliente = new AdminMultisoftware.Repositorio.Pessoas.Cliente(unitOfWorkAdmin);
            AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente = repCliente.BuscarPorCodigo(clienteCodigo);
            Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(unitOfWork.StringConexao, cliente, tipoServicoMultisoftware, stringConexaoAdmin);

            try
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregado repPagamentoAgregado = new Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregado(unitOfWork);
                Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoDocumento repPagamentoAgregadoDocumento = new Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoDocumento(unitOfWork);
                Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoInfracaoParcela repPagamentoAgregadoInfracaoParcela = new Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoInfracaoParcela(unitOfWork);
                Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoAbastecimento repPagamentoAgregadoAbastecimento = new Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoAbastecimento(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro repConfiguracaoContratoFreteTerceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro(unitOfWork);
                Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);
                Repositorio.Embarcador.Frota.InfracaoParcela repInfracaoParcela = new Repositorio.Embarcador.Frota.InfracaoParcela(unitOfWork);
                Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(unitOfWork);
                Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto repContratoFreteAcrescimoDesconto = new Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto(unitOfWork);
                Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoAcrescimoDesconto repPagamentoAgregadoAcrescimoDesconto = new Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoAcrescimoDesconto(unitOfWork);

                Repositorio.Embarcador.Documentos.CIOTCTe repCIOTCTe = new Repositorio.Embarcador.Documentos.CIOTCTe(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro configuracaoContratoFrete = repConfiguracaoContratoFreteTerceiro.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

                List<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregado> pagamentos = repPagamentoAgregado.BuscarPorStatus(Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPagamentoAgregado.AgInicio);

                foreach (var pagamento in pagamentos)
                {
                    usuarioPagamento = pagamento.Usuario;
                    codigoPagamento = pagamento.Codigo;

                    unitOfWork.Start();
                    pagamento.StatusPagamentoAgregado = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPagamentoAgregado.EmProcesso;
                    repPagamentoAgregado.Atualizar(pagamento);
                    unitOfWork.CommitChanges();

                    List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaDocumentos = repCTe.BuscarPorDocumentoAgregado(configuracaoContratoFrete.UtilizarNovoLayoutPagamentoAgregado, pagamento.Veiculo?.Codigo ?? 0, pagamento.DataInicialOcorrencia, pagamento.DataFinalOcorrencia, true, pagamento.TipoOcorrencia?.Codigo ?? 0, 0, pagamento.DataInicial.HasValue ? pagamento.DataInicial.Value : DateTime.MinValue, pagamento.DataFinal.HasValue ? pagamento.DataFinal.Value : DateTime.MinValue, 0, pagamento.Cliente.CPF_CNPJ_SemFormato, pagamento.Codigo, 0, "", "", 0, 0);
                    if (configuracaoContratoFrete.UtilizarNovoLayoutPagamentoAgregado)
                    {
                        List<Dominio.Entidades.Embarcador.Frota.InfracaoParcela> infracoes = repInfracaoParcela.BuscarPorDocumentoAgregado(pagamento.Veiculo?.Codigo ?? 0, pagamento.DataInicial.HasValue ? pagamento.DataInicial.Value : DateTime.MinValue, pagamento.DataFinal.HasValue ? pagamento.DataFinal.Value : DateTime.MinValue, pagamento.Cliente?.CPF_CNPJ ?? 0, pagamento.Codigo);
                        List<Dominio.Entidades.Abastecimento> abastecimentos = repAbastecimento.BuscarPorDocumentoAgregado(pagamento.Veiculo?.Codigo ?? 0, pagamento.DataInicial.HasValue ? pagamento.DataInicial.Value : DateTime.MinValue, pagamento.DataFinal.HasValue ? pagamento.DataFinal.Value : DateTime.MinValue, pagamento.Cliente?.CPF_CNPJ ?? 0, pagamento.Codigo);
                        if (infracoes != null && infracoes.Count > 0)
                        {
                            foreach (var infracao in infracoes)
                            {
                                if (!repPagamentoAgregadoInfracaoParcela.ContemInfracaoPagamento(pagamento.Codigo, infracao.Codigo))
                                {
                                    Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoInfracaoParcela pagamentoInfracao = new Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoInfracaoParcela()
                                    {
                                        InfracaoParcela = infracao,
                                        PagamentoAgregado = pagamento
                                    };
                                    repPagamentoAgregadoInfracaoParcela.Inserir(pagamentoInfracao);
                                }
                            }
                        }
                        if (abastecimentos != null && abastecimentos.Count > 0)
                        {
                            foreach (var abastecimento in abastecimentos)
                            {
                                if (!repPagamentoAgregadoAbastecimento.ContemAbastecimentoPagamento(pagamento.Codigo, abastecimento.Codigo))
                                {
                                    Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoAbastecimento abastecimentoInfracao = new Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoAbastecimento()
                                    {
                                        Abastecimento = abastecimento,
                                        PagamentoAgregado = pagamento
                                    };
                                    repPagamentoAgregadoAbastecimento.Inserir(abastecimentoInfracao);
                                }
                            }
                        }
                    }

                    serNotificacao.InfomarPercentualProcessamento(pagamento.Usuario, pagamento.Codigo, "PagamentosAgregados/PagamentoAgregado", 2, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.pagamentoAgregado, tipoServicoMultisoftware, unitOfWork);

                    int indiceLinha = 1;
                    int totalLinhas = listaDocumentos.Count() + 1;

                    foreach (var doc in listaDocumentos)
                    {

                        Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoDocumento documento = new Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoDocumento();

                        if (configuracaoContratoFrete.UtilizarNovoLayoutPagamentoAgregado)
                            documento.Valor = 0m;
                        else if (doc.PercentualPagamentoAgregado > 0 && doc.ValorFrete > 0)
                            documento.Valor = doc.ValorFrete * (doc.PercentualPagamentoAgregado / 100);
                        else
                            documento.Valor = Servicos.Embarcador.Carga.Frete.CalcularFretePorCTe(doc, unitOfWork, unitOfWork.StringConexao, tipoServicoMultisoftware, pagamento.Cliente.GrupoPessoas, configuracaoTMS);

                        unitOfWork.Start();

                        documento.PagamentoAgregado = pagamento;
                        documento.ConhecimentoDeTransporteEletronico = doc;
                        documento.StatusPagamentoAgregado = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPagamentoAgregado.Finalizado;
                        documento.CIOT = repCIOTCTe.BuscarPorCTe(doc.Codigo);
                        if (configuracaoContratoFrete.UtilizarNovoLayoutPagamentoAgregado)
                        {
                            if (documento.Valor <= 0 && documento.ConhecimentoDeTransporteEletronico != null)
                            {
                                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCTe.BuscarPorCTe(documento.ConhecimentoDeTransporteEletronico.Codigo);
                                if (cargaCTe != null)
                                {
                                    Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete = repContratoFrete.BuscarPorCarga(cargaCTe.Carga.Codigo);
                                    documento.ContratoFrete = contratoFrete;
                                    documento.Carga = cargaCTe.Carga;
                                    documento.Valor = (contratoFrete?.ValorFreteSubcontratacao ?? 0m) - (contratoFrete?.ValorImpostosReter ?? 0m);
                                    documento.ValorAdiantamento = contratoFrete?.ValorAdiantamento ?? 0m;
                                    documento.ValorSaldo = contratoFrete?.ValorSaldo ?? 0m;
                                }
                                else if (documento.CIOT != null)
                                {
                                    documento.Carga = documento.CIOT.CargaCIOT?.FirstOrDefault()?.Carga;
                                    documento.ContratoFrete = documento.Carga != null ? repContratoFrete.BuscarPorCarga(documento.Carga.Codigo) : null;
                                    documento.Valor = (documento.CIOT?.CargaCIOT?.Sum(o => (o.ContratoFrete?.ValorFreteSubcontratacao ?? 0m)) ?? 0m) - (documento.CIOT?.CargaCIOT?.Sum(o => (o.ContratoFrete?.ValorImpostosReter ?? 0m)) ?? 0m);
                                    documento.ValorAdiantamento = documento.CIOT?.CargaCIOT?.Sum(o => (o.ContratoFrete?.ValorAdiantamento ?? 0m)) ?? 0m;
                                    documento.ValorSaldo = documento.CIOT?.CargaCIOT?.Sum(o => (o.ContratoFrete?.ValorSaldo ?? 0m)) ?? 0m;
                                }
                            }
                            else if (documento.CIOT != null)
                            {
                                documento.Carga = documento.CIOT.CargaCIOT?.FirstOrDefault()?.Carga;
                                documento.ContratoFrete = documento.Carga != null ? repContratoFrete.BuscarPorCarga(documento.Carga.Codigo) : null;
                                documento.Valor = (documento.CIOT?.CargaCIOT?.Sum(o => (o.ContratoFrete?.ValorFreteSubcontratacao ?? 0m)) ?? 0m) - (documento.CIOT?.CargaCIOT?.Sum(o => (o.ContratoFrete?.ValorImpostosReter ?? 0m)) ?? 0m);
                                documento.ValorAdiantamento = documento.CIOT?.CargaCIOT?.Sum(o => (o.ContratoFrete?.ValorAdiantamento ?? 0m)) ?? 0m;
                                documento.ValorSaldo = documento.CIOT?.CargaCIOT?.Sum(o => (o.ContratoFrete?.ValorSaldo ?? 0m)) ?? 0m;
                            }
                        }

                        repPagamentoAgregadoDocumento.Inserir(documento);

                        if (configuracaoContratoFrete.UtilizarNovoLayoutPagamentoAgregado && documento.ContratoFrete != null)
                        {
                            List<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto> acrescimosDescontos = repContratoFreteAcrescimoDesconto.BuscarPorSituacaoEContratoSemPagamento(documento.ContratoFrete.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFreteAcrescimoDesconto.Finalizado);
                            if (acrescimosDescontos != null && acrescimosDescontos.Count > 0)
                            {
                                foreach (var acrescimoDesconto in acrescimosDescontos)
                                {
                                    Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoAcrescimoDesconto descontoAcrescimo = new Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoAcrescimoDesconto()
                                    {
                                        ContratoFreteAcrescimoDesconto = acrescimoDesconto,
                                        Justificativa = acrescimoDesconto.Justificativa,
                                        PagamentoAgregado = pagamento,
                                        Valor = acrescimoDesconto.Valor
                                    };
                                    repPagamentoAgregadoAcrescimoDesconto.Inserir(descontoAcrescimo);
                                }
                            }
                        }

                        pagamento.Valor += documento.Valor;
                        repPagamentoAgregado.Atualizar(pagamento);

                        unitOfWork.CommitChanges();

                        indiceLinha++;
                        int processados = (int)(100 * indiceLinha) / totalLinhas;
                        if (processados > 100)
                            processados = 100;
                        serNotificacao.InfomarPercentualProcessamento(pagamento.Usuario, pagamento.Codigo, "PagamentosAgregados/PagamentoAgregado", (decimal)processados, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.pagamentoAgregado, tipoServicoMultisoftware, unitOfWork);
                    }

                    unitOfWork.Start();

                    pagamento.StatusPagamentoAgregado = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPagamentoAgregado.Finalizado;
                    serNotificacao.GerarNotificacao(pagamento.Usuario, pagamento.Codigo, "PagamentosAgregados/PagamentoAgregado", Localization.Resources.Threads.PagamentoAgregado.ProcessoCalculoDocumentosConcluido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.sucesso, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.pagamentoAgregado, tipoServicoMultisoftware, unitOfWork);
                    repPagamentoAgregado.Atualizar(pagamento);

                    unitOfWork.CommitChanges();
                }


            }
            catch (Exception ex)
            {
                serNotificacao.GerarNotificacao(usuarioPagamento, codigoPagamento, "PagamentosAgregados/PagamentoAgregado", Localization.Resources.Threads.PagamentoAgregado.ProblemasProcessoCalculoDocumentosAgregado, Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.sucesso, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.pagamentoAgregado, tipoServicoMultisoftware, unitOfWork);
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
            }
        }

        private void ProcessarDocumentosManuaisPagamentoAgregado(Repositorio.UnitOfWork unitOfWork, UnitOfWork unitOfWorkAdmin, int codigoEmpresa, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string stringConexaoAdmin, int clienteCodigo)
        {
            Dominio.Entidades.Usuario usuarioPagamento = null;
            int codigoPagamento = 0;

            AdminMultisoftware.Repositorio.Pessoas.Cliente repCliente = new AdminMultisoftware.Repositorio.Pessoas.Cliente(unitOfWorkAdmin);
            AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente = repCliente.BuscarPorCodigo(clienteCodigo);
            Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(unitOfWork.StringConexao, cliente, tipoServicoMultisoftware, stringConexaoAdmin);
            try
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregado repPagamentoAgregado = new Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregado(unitOfWork);
                Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoDocumento repPagamentoAgregadoDocumento = new Repositorio.Embarcador.PagamentoAgregado.PagamentoAgregadoDocumento(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Documentos.CIOTCTe repCIOTCTe = new Repositorio.Embarcador.Documentos.CIOTCTe(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro repConfiguracaoContratoFreteTerceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro(unitOfWork);
                Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro configuracaoContratoFrete = repConfiguracaoContratoFreteTerceiro.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

                List<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregado> pagamentos = repPagamentoAgregado.BuscarPorStatus(Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPagamentoAgregado.AgInicioDocumentos);

                foreach (var pagamento in pagamentos)
                {
                    usuarioPagamento = pagamento.Usuario;
                    codigoPagamento = pagamento.Codigo;

                    unitOfWork.Start();
                    pagamento.StatusPagamentoAgregado = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPagamentoAgregado.EmProcesso;
                    repPagamentoAgregado.Atualizar(pagamento);
                    unitOfWork.CommitChanges();

                    List<Dominio.Entidades.Embarcador.PagamentoAgregado.PagamentoAgregadoDocumento> listaDocumentos = repPagamentoAgregadoDocumento.BuscarPorStatus(Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPagamentoAgregado.AgInicio);

                    serNotificacao.InfomarPercentualProcessamento(pagamento.Usuario, pagamento.Codigo, "PagamentosAgregados/PagamentoAgregado", 2, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.pagamentoAgregado, tipoServicoMultisoftware, unitOfWork);

                    int indiceLinha = 1;
                    int totalLinhas = listaDocumentos.Count() + 1;

                    foreach (var doc in listaDocumentos)
                    {
                        if (doc.ConhecimentoDeTransporteEletronico.PercentualPagamentoAgregado > 0 && doc.ConhecimentoDeTransporteEletronico.ValorFrete > 0)
                            doc.Valor = doc.ConhecimentoDeTransporteEletronico.ValorFrete * (doc.ConhecimentoDeTransporteEletronico.PercentualPagamentoAgregado / 100);
                        else
                            doc.Valor = Servicos.Embarcador.Carga.Frete.CalcularFretePorCTe(doc.ConhecimentoDeTransporteEletronico, unitOfWork, unitOfWork.StringConexao, tipoServicoMultisoftware, pagamento.Cliente.GrupoPessoas, configuracaoTMS);

                        doc.CIOT = repCIOTCTe.BuscarPorCTe(doc.Codigo);
                        if (configuracaoContratoFrete.UtilizarNovoLayoutPagamentoAgregado)
                        {
                            if (doc.CIOT != null)
                            {
                                doc.Valor = (doc.CIOT?.CargaCIOT?.Sum(o => (o.ContratoFrete?.ValorFreteSubcontratacao ?? 0m)) ?? 0m) - (doc.CIOT?.CargaCIOT?.Sum(o => (o.ContratoFrete?.ValorImpostosReter ?? 0m)) ?? 0m);
                                doc.ValorAdiantamento = doc.CIOT?.CargaCIOT?.Sum(o => (o.ContratoFrete?.ValorAdiantamento ?? 0m)) ?? 0m;
                                doc.ValorSaldo = doc.CIOT?.CargaCIOT?.Sum(o => (o.ContratoFrete?.ValorSaldo ?? 0m)) ?? 0m;
                            }

                            if (doc.Valor <= 0 && doc.ConhecimentoDeTransporteEletronico != null)
                            {
                                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCTe.BuscarPorCTe(doc.ConhecimentoDeTransporteEletronico.Codigo);
                                if (cargaCTe != null)
                                {
                                    Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete = repContratoFrete.BuscarPorCarga(cargaCTe.Carga.Codigo);
                                    doc.Valor = (contratoFrete?.ValorFreteSubcontratacao ?? 0m) - (contratoFrete?.ValorImpostosReter ?? 0m);
                                    doc.ValorAdiantamento = contratoFrete?.ValorAdiantamento ?? 0m;
                                    doc.ValorSaldo = contratoFrete?.ValorSaldo ?? 0m;
                                }
                            }
                        }

                        unitOfWork.Start();

                        doc.StatusPagamentoAgregado = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPagamentoAgregado.Finalizado;
                        repPagamentoAgregadoDocumento.Atualizar(doc);

                        pagamento.Valor += doc.Valor;
                        repPagamentoAgregado.Atualizar(pagamento);

                        unitOfWork.CommitChanges();

                        indiceLinha++;
                        int processados = (int)(100 * indiceLinha) / totalLinhas;
                        if (processados > 100)
                            processados = 100;
                        serNotificacao.InfomarPercentualProcessamento(pagamento.Usuario, pagamento.Codigo, "PagamentosAgregados/PagamentoAgregado", (decimal)processados, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.pagamentoAgregado, tipoServicoMultisoftware, unitOfWork);
                    }

                    unitOfWork.Start();
                    pagamento.StatusPagamentoAgregado = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPagamentoAgregado.Finalizado;
                    serNotificacao.GerarNotificacao(pagamento.Usuario, pagamento.Codigo, "PagamentosAgregados/PagamentoAgregado", Localization.Resources.Threads.PagamentoAgregado.ProcessoCalculoDocumentosConcluido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.sucesso, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.pagamentoAgregado, tipoServicoMultisoftware, unitOfWork);
                    repPagamentoAgregado.Atualizar(pagamento);
                    unitOfWork.CommitChanges();
                }

            }
            catch (Exception ex)
            {
                serNotificacao.GerarNotificacao(usuarioPagamento, codigoPagamento, "PagamentosAgregados/PagamentoAgregado", Localization.Resources.Threads.PagamentoAgregado.ProblemasProcessoCalculoDocumentosAgregado, Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.sucesso, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.pagamentoAgregado, tipoServicoMultisoftware, unitOfWork);
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
            }
        }
    }
}