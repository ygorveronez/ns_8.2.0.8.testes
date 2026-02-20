using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Newtonsoft.Json;
using SGT.BackgroundWorkers.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 60000)]

    public class ProcessarRetornoAprovacaoTabelaFrete : LongRunningProcessBase<ProcessarRetornoAprovacaoTabelaFrete>
    {
        #region Atributos

        private Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        #region Métodos Públicos

        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            _unitOfWork = unitOfWork;

            Processar();
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private void Processar()
        {
            Repositorio.Embarcador.Frete.ProcessamentoAprovacaoTabelaAssincrono repProcessamentoAprovacaoTabelaAssincrono = new Repositorio.Embarcador.Frete.ProcessamentoAprovacaoTabelaAssincrono(_unitOfWork);
            try
            {
                List<Dominio.Entidades.Embarcador.Frete.ProcessamentoAprovacaoTabelaAssincrono> listaProcessamentoAprovacaoTabelaAssincrono = repProcessamentoAprovacaoTabelaAssincrono.BuscarProcessamentoPendentes();
                if (listaProcessamentoAprovacaoTabelaAssincrono != null && listaProcessamentoAprovacaoTabelaAssincrono.Count > 0)
                {
                    bool sucessoProcessamento = false;
                    foreach (Dominio.Entidades.Embarcador.Frete.ProcessamentoAprovacaoTabelaAssincrono processamentoAprovacaoTabelaAssincrono in listaProcessamentoAprovacaoTabelaAssincrono)
                    {
                        sucessoProcessamento = false;
                        processamentoAprovacaoTabelaAssincrono.Tentativas += 1;
                        repProcessamentoAprovacaoTabelaAssincrono.Atualizar(processamentoAprovacaoTabelaAssincrono);

                        _unitOfWork.Start();

                        string request = Servicos.Embarcador.Integracao.ArquivoIntegracao.RetornarArquivoTexto(processamentoAprovacaoTabelaAssincrono.ArquivoRequisicao);
                        List<Dominio.ObjetosDeValor.WebService.Frete.RetornoTabelaFrete> retornosTabelaFrete = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.WebService.Frete.RetornoTabelaFrete>>(request);

                        sucessoProcessamento = ProcessarRetornoAprovacaoTabela(retornosTabelaFrete, _auditado);

                        processamentoAprovacaoTabelaAssincrono.Situacao = sucessoProcessamento ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.Frete.SituacaoProcessamentoAprovacaoTabelaAssincrono.Processado : Dominio.ObjetosDeValor.Embarcador.Enumeradores.Frete.SituacaoProcessamentoAprovacaoTabelaAssincrono.FalhaNoProcessamento;
                        processamentoAprovacaoTabelaAssincrono.DataProcessamento = DateTime.Now;

                        repProcessamentoAprovacaoTabelaAssincrono.Atualizar(processamentoAprovacaoTabelaAssincrono);

                        _unitOfWork.CommitChanges();
                    }
                }

                _unitOfWork.FlushAndClear();
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
            }
        }

        private bool ProcessarRetornoAprovacaoTabela(List<Dominio.ObjetosDeValor.WebService.Frete.RetornoTabelaFrete> retornosTabelaFrete, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditoria)
        {
            List<int> codigosIntegracaoFrete = retornosTabelaFrete.Select(retornoIntegracao => retornoIntegracao.IDTarifaMulti.ToInt()).ToList();
            Repositorio.Embarcador.Cargas.TipoIntegracao repostitorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Frete.IntegracaoFrete repositorioIntegracaoFrete = new Repositorio.Embarcador.Frete.IntegracaoFrete(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracaoLBC = repostitorioTipoIntegracao.BuscarPorTipo(TipoIntegracao.LBC);
            List<Dominio.Entidades.Embarcador.Frete.IntegracaoFrete> integracoesFrete = repositorioIntegracaoFrete.BuscarPorCodigos(codigosIntegracaoFrete);
            List<Dominio.Entidades.Embarcador.Frete.IntegracaoFrete> integracoesTabelaFreteCliente = integracoesFrete.Where(integracao => integracao.Tipo == TipoIntegracaoFrete.TabelaFreteCliente).ToList();
            List<Dominio.Entidades.Embarcador.Frete.IntegracaoFrete> integracoesContratoFreteTransportador = integracoesFrete.Where(integracao => integracao.Tipo == TipoIntegracaoFrete.ContratoFreteTransportador).ToList();

            List<string> mensagensErro = new List<string>();
            List<int> codigoTabelafreteCliente = integracoesTabelaFreteCliente.Select(x => x.Codigo).ToList();
            List<int> codigoTabelafreteTransportador = integracoesContratoFreteTransportador.Select(x => x.Codigo).ToList();

            int take = 1000;
            int start = 0;
            List<Dominio.ObjetosDeValor.WebService.Frete.RetornoTabelaFrete> retornoIntegracoesTabelaFreteCliente = new List<Dominio.ObjetosDeValor.WebService.Frete.RetornoTabelaFrete>();
            while (start < retornosTabelaFrete?.Count)
            {
                List<Dominio.ObjetosDeValor.WebService.Frete.RetornoTabelaFrete> tmp = retornosTabelaFrete.Skip(start).Take(take).ToList();

                List<Dominio.ObjetosDeValor.WebService.Frete.RetornoTabelaFrete> retorno = tmp.Where(retornoIntegracao => codigoTabelafreteCliente.Contains(retornoIntegracao.IDTarifaMulti.ToInt())).ToList();
                retornoIntegracoesTabelaFreteCliente.AddRange(retorno);
                start += take;
            }

            int takeTransportador = 1000;
            int startTransportador = 0;
            List<Dominio.ObjetosDeValor.WebService.Frete.RetornoTabelaFrete> retornoIntegracoesContratoFreteTransportador = new List<Dominio.ObjetosDeValor.WebService.Frete.RetornoTabelaFrete>();
            while (startTransportador < retornosTabelaFrete?.Count)
            {
                List<Dominio.ObjetosDeValor.WebService.Frete.RetornoTabelaFrete> tmp2 = retornosTabelaFrete.Skip(startTransportador).Take(takeTransportador).ToList();

                List<Dominio.ObjetosDeValor.WebService.Frete.RetornoTabelaFrete> retorno = tmp2.Where(retornoIntegracao => codigoTabelafreteTransportador.Contains(retornoIntegracao.IDTarifaMulti.ToInt())).ToList();
                retornoIntegracoesContratoFreteTransportador.AddRange(retorno);
                startTransportador += takeTransportador;
            }
            List<string> codigosRetornosIntegracaoIdentificados = retornoIntegracoesTabelaFreteCliente.Select(retornoIntegracao => retornoIntegracao.IDTarifaMulti).Concat(retornoIntegracoesContratoFreteTransportador.Select(retornoIntegracao => retornoIntegracao.IDTarifaMulti)).ToList();

            int takeNaoIdentificados = 1000;
            int startNaoIdentificados = 0;
            List<Dominio.ObjetosDeValor.WebService.Frete.RetornoTabelaFrete> retornoIntegracoesNaoIdentificados = new List<Dominio.ObjetosDeValor.WebService.Frete.RetornoTabelaFrete>();
            while (startNaoIdentificados < retornosTabelaFrete?.Count)
            {
                List<Dominio.ObjetosDeValor.WebService.Frete.RetornoTabelaFrete> tmp2 = retornosTabelaFrete.Skip(startNaoIdentificados).Take(takeNaoIdentificados).ToList();

                List<Dominio.ObjetosDeValor.WebService.Frete.RetornoTabelaFrete> retorno = tmp2.Where(retornoIntegracao => !codigosRetornosIntegracaoIdentificados.Contains(retornoIntegracao.IDTarifaMulti)).ToList();
                retornoIntegracoesNaoIdentificados.AddRange(retorno);
                startNaoIdentificados += takeNaoIdentificados;
            }

            RetornoAprovacaoTabelaFreteCliente(retornoIntegracoesTabelaFreteCliente, integracoesTabelaFreteCliente, tipoIntegracaoLBC, mensagensErro);
            RetornoAprovacaoContratoFreteTransportador(retornoIntegracoesContratoFreteTransportador, integracoesContratoFreteTransportador, tipoIntegracaoLBC, mensagensErro, auditoria);

            foreach (Dominio.ObjetosDeValor.WebService.Frete.RetornoTabelaFrete retornoIntegracaoNaoIdentificado in retornoIntegracoesNaoIdentificados)
                mensagensErro.Add($"{retornoIntegracaoNaoIdentificado.IDTarifaMulti} - Não foi possível encontrar uma tarifa de frete");

            if (mensagensErro.Count > 0)
            {
                Servicos.Log.TratarErro($"Valores com problema na integração", "RetornoAprovacaoTabela");
                return false;
            }

            return true;
        }

        private void RetornoAprovacaoContratoFreteTransportador(List<Dominio.ObjetosDeValor.WebService.Frete.RetornoTabelaFrete> retornosContratoFreteTransportador, List<Dominio.Entidades.Embarcador.Frete.IntegracaoFrete> integracoes, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracaoLBC, List<string> mensagensErro, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditoria)
        {
            if (retornosContratoFreteTransportador.Count == 0)
                return;

            List<int> codigosContratos = integracoes.Select(integracao => integracao.CodigoIntegracao).ToList();
            Repositorio.Embarcador.Frete.ContratoFreteTransportador repositorioContratoTransporteFrete = new Repositorio.Embarcador.Frete.ContratoFreteTransportador(_unitOfWork);
            Repositorio.Embarcador.Frete.ContratoFreteTransportadorIntegracao repositorioContratoTransporteFreteIntegracao = new Repositorio.Embarcador.Frete.ContratoFreteTransportadorIntegracao(_unitOfWork);
            Repositorio.Embarcador.Frete.IntegracaoFrete repositorioIntegracaoFrete = new Repositorio.Embarcador.Frete.IntegracaoFrete(_unitOfWork);
            Repositorio.Embarcador.Frete.StatusAssinaturaContrato repositorioStatusAssinaturaContrato = new Repositorio.Embarcador.Frete.StatusAssinaturaContrato(_unitOfWork);
            Servicos.Embarcador.Integracao.ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new Servicos.Embarcador.Integracao.ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador> contratosFreteTransportador = repositorioContratoTransporteFrete.BuscarPorCodigos(codigosContratos);

            foreach (Dominio.ObjetosDeValor.WebService.Frete.RetornoTabelaFrete retornoContratoFreteTransportador in retornosContratoFreteTransportador)
            {
                try
                {
                    Dominio.Entidades.Embarcador.Frete.IntegracaoFrete integracaoFrete = integracoes.Where(integracao => integracao.Codigo == retornoContratoFreteTransportador.IDTarifaMulti.ToInt()).FirstOrDefault();
                    Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contratoFreteTransportador = contratosFreteTransportador.Where(o => o.Codigo == integracaoFrete.CodigoIntegracao).FirstOrDefault();

                    if (contratoFreteTransportador == null)
                        throw new ServicoException($"Não foi possível encontrar o contrato de frete de transportador");

                    integracaoFrete.CodigoRetornoIntegracao = retornoContratoFreteTransportador.IDTarifaJaggaer;
                    contratoFreteTransportador.IDExterno = retornoContratoFreteTransportador.IDTarifaJaggaer;
                    contratoFreteTransportador.StatusAceiteContrato = repositorioStatusAssinaturaContrato.BuscarPorCodigoIntegracao(retornoContratoFreteTransportador.Status);

                    if (contratoFreteTransportador.StatusAceiteContrato == null)
                        throw new ServicoException($"Status {retornoContratoFreteTransportador.Status} não cadastrado na multisoftware");

                    repositorioContratoTransporteFrete.Atualizar(contratoFreteTransportador);
                    repositorioIntegracaoFrete.Atualizar(integracaoFrete);

                    if (tipoIntegracaoLBC != null)
                    {
                        Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorIntegracao contratoFreteTransportadorIntegracao = repositorioContratoTransporteFreteIntegracao.BuscarAguardandoRetornoPorTipoIntegracao(contratoFreteTransportador.Codigo, tipoIntegracaoLBC.Codigo);

                        if (contratoFreteTransportadorIntegracao != null)
                        {
                            if ((retornoContratoFreteTransportador.Status == "A") || (retornoContratoFreteTransportador.Status == "J"))
                            {
                                contratoFreteTransportadorIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                                repositorioContratoTransporteFreteIntegracao.Atualizar(contratoFreteTransportadorIntegracao);

                                if (
                                    (retornoContratoFreteTransportador.Status == "A") &&
                                    (contratoFreteTransportador.ContratoOriginario != null) &&
                                    (contratoFreteTransportador.ContratoOriginario.DataInicial < contratoFreteTransportador.DataInicial) &&
                                    (contratoFreteTransportador.ContratoOriginario.DataFinal >= contratoFreteTransportador.DataInicial)
                                )
                                {
                                    DateTime dataFinalAnterior = contratoFreteTransportador.ContratoOriginario.DataFinal;

                                    contratoFreteTransportador.ContratoOriginario.DataFinal = contratoFreteTransportador.DataInicial.AddDays(-1);

                                    repositorioContratoTransporteFrete.Atualizar(contratoFreteTransportador.ContratoOriginario);
                                    Servicos.Auditoria.Auditoria.Auditar(auditoria, contratoFreteTransportador.ContratoOriginario, $"Alterada a data final de {dataFinalAnterior:dd/MM/yyyy} para {contratoFreteTransportador.ContratoOriginario.DataFinal:dd/MM/yyyy} ao aprovar o contrato {contratoFreteTransportador.Numero}", _unitOfWork);
                                }
                            }

                            string jsonRequisicao = string.Empty;
                            string jsonRetorno = JsonConvert.SerializeObject(retornoContratoFreteTransportador, Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });

                            servicoArquivoTransacao.Adicionar(contratoFreteTransportadorIntegracao, jsonRequisicao, jsonRetorno, "json");
                        }
                    }
                }
                catch (ServicoException excecao)
                {
                    mensagensErro.Add($"{retornoContratoFreteTransportador.IDTarifaMulti} - {excecao.Message}");
                }
            }
        }

        private void RetornoAprovacaoTabelaFreteCliente(List<Dominio.ObjetosDeValor.WebService.Frete.RetornoTabelaFrete> retornosTabelaFreteCliente, List<Dominio.Entidades.Embarcador.Frete.IntegracaoFrete> integracoes, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracaoLBC, List<string> mensagensErro)
        {
            if (retornosTabelaFreteCliente.Count == 0)
                return;

            List<int> codigosItens = integracoes.Select(integracao => integracao.CodigoIntegracao).ToList();
            Repositorio.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete repositorioValores = new Repositorio.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete(_unitOfWork);
            Repositorio.Embarcador.Frete.IntegracaoFrete repositorioIntegracaoFrete = new Repositorio.Embarcador.Frete.IntegracaoFrete(_unitOfWork);
            Repositorio.Embarcador.Frete.StatusAssinaturaContrato repositorioStatusAssinaturaContrato = new Repositorio.Embarcador.Frete.StatusAssinaturaContrato(_unitOfWork);
            Repositorio.Embarcador.Frete.TabelaFreteClienteModeloVeicularCarga repositorioTabelaFreteClienteModeloVeicularCarga = new Repositorio.Embarcador.Frete.TabelaFreteClienteModeloVeicularCarga(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete> itens = repositorioValores.BuscarPorCodigos(codigosItens);
            List<(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente TabelaFreteCliente, Dominio.ObjetosDeValor.WebService.Frete.RetornoTabelaFrete RetornoTabelaFrete)> retornosPorTabelaFreteCliente = new List<(Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente TabelaFreteCliente, Dominio.ObjetosDeValor.WebService.Frete.RetornoTabelaFrete RetornoTabelaFrete)>();

            foreach (Dominio.ObjetosDeValor.WebService.Frete.RetornoTabelaFrete retornoTabelaFreteCliente in retornosTabelaFreteCliente)
            {
                try
                {
                    Dominio.Entidades.Embarcador.Frete.IntegracaoFrete integracaoFrete = integracoes.Where(integracao => integracao.Codigo == retornoTabelaFreteCliente.IDTarifaMulti.ToInt()).FirstOrDefault();
                    Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete item = itens.Where(o => o.Codigo == integracaoFrete.CodigoIntegracao).FirstOrDefault();

                    if (item == null)
                        throw new ServicoException($"Não foi possível encontrar o item");

                    retornosPorTabelaFreteCliente.Add(ValueTuple.Create(item.ParametroBaseCalculo.TabelaFrete, retornoTabelaFreteCliente));

                    if (item.Situacao != SituacaoItemParametroBaseCalculoTabelaFrete.AguardandoRetornoIntegracao)
                        throw new ServicoException($"O item já teve seu retorno enviado");

                    integracaoFrete.CodigoRetornoIntegracao = retornoTabelaFreteCliente.IDTarifaJaggaer;
                    item.StatusAceiteValor = repositorioStatusAssinaturaContrato.BuscarPorCodigoIntegracao(retornoTabelaFreteCliente.Status);

                    if (item.StatusAceiteValor == null)
                        throw new ServicoException($"Status {retornoTabelaFreteCliente.Status} não cadastrado na multisoftware");

                    if ((retornoTabelaFreteCliente.Status == "J"))
                    {
                        decimal valorOriginal = item.Valor;

                        item.Valor = item.ValorOriginal;
                        item.ValorOriginal = valorOriginal;
                    }

                    if ((retornoTabelaFreteCliente.Status == "A") || (retornoTabelaFreteCliente.Status == "J"))
                    {
                        item.Situacao = SituacaoItemParametroBaseCalculoTabelaFrete.Ativo;

                        repositorioTabelaFreteClienteModeloVeicularCarga.RemoverPendenciaIntegracao(item.ParametroBaseCalculo.TabelaFrete.Codigo, item.ParametroBaseCalculo.CodigoObjeto);
                        repositorioValores.RemoverPendenciaIntegracaoPorParamentroBase(item.ParametroBaseCalculo.Codigo);
                    }

                    repositorioValores.Atualizar(item);
                    repositorioIntegracaoFrete.Atualizar(integracaoFrete);
                }
                catch (ServicoException excecao)
                {
                    mensagensErro.Add($"{retornoTabelaFreteCliente.IDTarifaMulti} - {excecao.Message}");
                }
            }

            if (tipoIntegracaoLBC == null)
                return;

            List<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente> tabelasFreteCliente = retornosPorTabelaFreteCliente.Select(o => o.TabelaFreteCliente).Distinct().ToList();

            if (tabelasFreteCliente.Count == 0)
                return;

            Repositorio.Embarcador.Frete.TabelaFreteClienteIntegracao repositorioTabelaFreteIntegracao = new Repositorio.Embarcador.Frete.TabelaFreteClienteIntegracao(_unitOfWork);
            Servicos.Embarcador.Frete.TabelaFreteClienteIntegracao servicoTabelaFreteClienteIntegracao = new Servicos.Embarcador.Frete.TabelaFreteClienteIntegracao(_unitOfWork);
            Servicos.Embarcador.Integracao.ArquivoTransacao<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteArquivo> servicoArquivoTransacao = new Servicos.Embarcador.Integracao.ArquivoTransacao<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteArquivo>(_unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente in tabelasFreteCliente)
            {
                Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteIntegracao tabelaFreteClienteIntegracao = repositorioTabelaFreteIntegracao.BuscarAguardandoRetornoPorTipoIntegracao(tabelaFreteCliente.Codigo, tipoIntegracaoLBC.Codigo);

                if (tabelaFreteClienteIntegracao == null)
                    continue;

                List<Dominio.ObjetosDeValor.WebService.Frete.RetornoTabelaFrete> dadosRetorno = retornosPorTabelaFreteCliente.Where(o => o.TabelaFreteCliente.Codigo == tabelaFreteCliente.Codigo).Select(o => o.RetornoTabelaFrete).ToList();

                if (repositorioValores.TodosItensIntegradosPorTabelaFreteCliente(tabelaFreteCliente.Codigo))
                {
                    Servicos.Embarcador.Frete.AjusteTabelaFrete servicoAjusteTabelaFrete = new Servicos.Embarcador.Frete.AjusteTabelaFrete(_unitOfWork);
                    bool alteracaoVigenciaAprovada = dadosRetorno.Any(retorno => retorno.Status == "A");

                    tabelaFreteClienteIntegracao.DataIntegracao = DateTime.Now;
                    tabelaFreteClienteIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    tabelaFreteClienteIntegracao.ProblemaIntegracao = "Integração retornada com sucesso";

                    repositorioTabelaFreteIntegracao.Atualizar(tabelaFreteClienteIntegracao);
                    servicoTabelaFreteClienteIntegracao.AtualizarSituacaoIntegracao(tabelaFreteCliente);
                    servicoAjusteTabelaFrete.FinalizarAlteracoesVigenciaPendente(tabelaFreteCliente, alteracaoVigenciaAprovada);
                }
                else
                {
                    tabelaFreteClienteIntegracao.DataIntegracao = DateTime.Now;

                    repositorioTabelaFreteIntegracao.Atualizar(tabelaFreteClienteIntegracao);
                }

                string jsonRequisicao = string.Empty;
                string jsonRetorno = JsonConvert.SerializeObject(dadosRetorno, Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                string mensagem = (tabelaFreteClienteIntegracao.SituacaoIntegracao == SituacaoIntegracao.Integrado) ? "Integração finalizada com sucesso" : "Integração de status parcial";

                servicoArquivoTransacao.Adicionar(tabelaFreteClienteIntegracao, jsonRequisicao, jsonRetorno, "json", mensagem, TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento);
            }
        }

        #endregion Métodos Privados
    }
}
