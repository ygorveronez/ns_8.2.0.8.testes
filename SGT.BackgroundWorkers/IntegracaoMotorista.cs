using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    public class IntegracaoMotorista : LongRunningProcessBase<IntegracaoMotorista>
    {
        #region Métodos Sobreescritos

        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            Servicos.Embarcador.Carga.Carga.ProcessarCargaSVMEmProcessamentoDocumentosFiscais(unitOfWork, _stringConexao, _tipoServicoMultisoftware);

            await Servicos.Embarcador.Integracao.Buonny.IntegracaoBuonny.GetInstance().ConsultarSMBuonnyAsync(unitOfWork, cancellationToken);
            GerarIntegracoesBuonnyVeiculosMotoristas(unitOfWork);
            VerificarIntegracoesBuonnyAntigas(unitOfWork);
            GerarIntegracaoTelerisco(unitOfWork);

            GerarIntegracaoMotoristaBrasilRisk(unitOfWork);
            GerarIntegracaoVeiculoBrasilRisk(unitOfWork);

            IntegrarMotoristaBrasilRiskAguardandoRetorno(unitOfWork);
            IntegrarVeiculoBrasilRiskAguardandoRetorno(unitOfWork);

            GerarIntegracaoUltragaz(unitOfWork);
            GerarIntegracaoFrota162(unitOfWork);
            GerarIntegracaoKMM(unitOfWork);
            new Servicos.Embarcador.Pessoa.PessoaIntegracao(unitOfWork).ProcessarIntegracoesPendentesPessoa();
        }

        #endregion Métodos Sobreescritos

        #region Métodos Públicos

        public void VerificarIntegracoesBuonnyAntigas(Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unidadeTrabalho);
            Repositorio.Embarcador.Veiculos.VeiculoIntegracao repVeiculoIntegracao = new Repositorio.Embarcador.Veiculos.VeiculoIntegracao(unidadeTrabalho);
            Repositorio.Embarcador.Transportadores.MotoristaIntegracao repMotoristaIntegracao = new Repositorio.Embarcador.Transportadores.MotoristaIntegracao(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repIntegracao.Buscar();

            int tempoConsultaHoras = configuracaoIntegracao != null ? configuracaoIntegracao.TempoHorasConsultasBuonny : 0;
            if (tempoConsultaHoras > 0)
            {
                tempoConsultaHoras = tempoConsultaHoras * -1;

                List<Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao> listaVeiculosIntegracaoMotorista = repVeiculoIntegracao.BuscarPendentesIntegracaoPorDataHora(DateTime.Now.AddHours(tempoConsultaHoras), Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Buonny);
                for (var i = 0; i < listaVeiculosIntegracaoMotorista.Count; i++)
                {
                    var integracao = listaVeiculosIntegracaoMotorista[i];
                    //integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                    //integracao.NumeroTentativas = 0;
                    //integracao.ProblemaIntegracao = string.Empty;
                    //repVeiculoIntegracao.Atualizar(integracao);
                    integracao.DataIntegracao = DateTime.Now;
                    integracao.NumeroTentativas = 0;
                    integracao.ProblemaIntegracao = string.Empty;

                    try
                    {
                        string mensagemErro = string.Empty;
                        Dominio.ObjetosDeValor.Embarcador.Integracao.Buonny.RetornoStatusChecklist retorno = Servicos.Embarcador.Integracao.Buonny.IntegracaoBuonny.StatusChecklist(integracao.Veiculo.Placa, ref mensagemErro, unidadeTrabalho);
                        if (!string.IsNullOrWhiteSpace(mensagemErro))
                        {
                            integracao.ProblemaIntegracao = mensagemErro.Length > 300 ? mensagemErro.Substring(0, 300) : mensagemErro;
                            integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                        }
                        else if (retorno == null)
                        {
                            integracao.ProblemaIntegracao = "Integração não teve retorno.";
                            integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                        }
                        else if (!string.IsNullOrWhiteSpace(retorno.status) && retorno.status.ToUpper() == "S")
                        {
                            integracao.ProblemaIntegracao = "Checklist valido " + retorno.data_checklist;
                            integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                        }
                        else
                        {
                            integracao.ProblemaIntegracao = "Veículo não possui checklist valido na Buonny.";
                            integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                        }

                        string stringRetorno = string.Empty;
                        if (retorno != null)
                            stringRetorno = Newtonsoft.Json.JsonConvert.SerializeObject(retorno);

                        Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo integracaoArquivo = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                        integracaoArquivo.Mensagem = !string.IsNullOrWhiteSpace(stringRetorno) ? Utilidades.String.Left(stringRetorno, 400) : integracao.ProblemaIntegracao;
                        integracaoArquivo.Data = DateTime.Now;
                        integracaoArquivo.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento;
                        repCargaCTeIntegracaoArquivo.Inserir(integracaoArquivo);

                        if (integracao.ArquivosTransacao == null)
                            integracao.ArquivosTransacao = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();
                        integracao.ArquivosTransacao.Add(integracaoArquivo);
                    }
                    catch (Exception excecao)
                    {
                        Servicos.Log.TratarErro(excecao);
                        integracao.ProblemaIntegracao = Utilidades.String.Left(excecao.Message, 300);
                        integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    }

                    repVeiculoIntegracao.Atualizar(integracao);
                }

                List<Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao> listaVeiculosIntegracaoMotoristaRNTRC = repVeiculoIntegracao.BuscarPendentesIntegracaoPorDataHora(DateTime.Now.AddHours(tempoConsultaHoras), Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.BuonnyRNTRC);
                for (var i = 0; i < listaVeiculosIntegracaoMotoristaRNTRC.Count; i++)
                {
                    var integracao = listaVeiculosIntegracaoMotoristaRNTRC[i];
                    //integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                    //integracao.NumeroTentativas = 0;
                    //integracao.ProblemaIntegracao = string.Empty;
                    //repVeiculoIntegracao.Atualizar(integracao);
                    integracao.DataIntegracao = DateTime.Now;
                    integracao.NumeroTentativas = 0;
                    integracao.ProblemaIntegracao = string.Empty;

                    try
                    {
                        string mensagemErro = string.Empty;
                        Dominio.ObjetosDeValor.Embarcador.Integracao.Buonny.RetornoStatusRNTRC retorno = Servicos.Embarcador.Integracao.Buonny.IntegracaoBuonny.StatusRNTRC(integracao.Veiculo.Placa, ref mensagemErro, unidadeTrabalho);
                        if (!string.IsNullOrWhiteSpace(mensagemErro))
                        {
                            integracao.ProblemaIntegracao = mensagemErro.Length > 300 ? mensagemErro.Substring(0, 300) : mensagemErro;
                            integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                        }
                        else if (retorno == null)
                        {
                            integracao.ProblemaIntegracao = "Integração não teve retorno.";
                            integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                        }
                        else if (!string.IsNullOrWhiteSpace(retorno.validado) && retorno.validado.ToUpper() == "S")
                        {
                            integracao.ProblemaIntegracao = "RNTRC valida.";
                            integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                        }
                        else
                        {
                            integracao.ProblemaIntegracao = "Veículo não possui RNTRC valida na Buonny.";
                            integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                        }

                        string stringRetorno = string.Empty;
                        if (retorno != null)
                            stringRetorno = Newtonsoft.Json.JsonConvert.SerializeObject(retorno);

                        Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo integracaoArquivo = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                        integracaoArquivo.Mensagem = !string.IsNullOrWhiteSpace(stringRetorno) ? Utilidades.String.Left(stringRetorno, 400) : integracao.ProblemaIntegracao;
                        integracaoArquivo.Data = DateTime.Now;
                        integracaoArquivo.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento;
                        repCargaCTeIntegracaoArquivo.Inserir(integracaoArquivo);

                        if (integracao.ArquivosTransacao == null)
                            integracao.ArquivosTransacao = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();
                        integracao.ArquivosTransacao.Add(integracaoArquivo);
                    }
                    catch (Exception excecao)
                    {
                        Servicos.Log.TratarErro(excecao);
                        integracao.ProblemaIntegracao = Utilidades.String.Left(excecao.Message, 300);
                        integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    }

                    repVeiculoIntegracao.Atualizar(integracao);
                }

                List<Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao> listaMotoristaIntegracao = repMotoristaIntegracao.BuscarPendentesIntegracaoPorDataHora(DateTime.Now.AddHours(tempoConsultaHoras), Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Buonny);
                for (var i = 0; i < listaMotoristaIntegracao.Count; i++)
                {
                    var integracao = listaMotoristaIntegracao[i];

                    integracao.DataIntegracao = DateTime.Now;
                    integracao.NumeroTentativas = 0;
                    integracao.ProblemaIntegracao = string.Empty;

                    try
                    {
                        Servicos.Embarcador.Integracao.Buonny.IntegracaoBuonny.StatusMotorista(ref integracao, _tipoServicoMultisoftware, unidadeTrabalho);
                    }
                    catch (Exception excecao)
                    {
                        Servicos.Log.TratarErro(excecao);
                        integracao.ProblemaIntegracao = Utilidades.String.Left(excecao.Message, 300);
                        integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    }

                    repMotoristaIntegracao.Atualizar(integracao);
                }
            }
        }

        public void GerarIntegracoesBuonnyVeiculosMotoristas(Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Veiculos.VeiculoIntegracao repVeiculoIntegracao = new Repositorio.Embarcador.Veiculos.VeiculoIntegracao(unidadeTrabalho);
            Repositorio.Embarcador.Transportadores.MotoristaIntegracao repMotoristaIntegracao = new Repositorio.Embarcador.Transportadores.MotoristaIntegracao(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unidadeTrabalho);

            int numeroTentativas = 2;
            double minutosACadaTentativa = 1;

            var tipoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Buonny;
            List<Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao> listaMotoristaIntegracao = repMotoristaIntegracao.BuscarPendentesIntegracaoPorTipo(numeroTentativas, minutosACadaTentativa, tipoIntegracao);
            for (var i = 0; i < listaMotoristaIntegracao.Count; i++)
            {
                var integracao = listaMotoristaIntegracao[i];

                integracao.DataIntegracao = DateTime.Now;
                integracao.NumeroTentativas += 1;

                try
                {
                    Servicos.Embarcador.Integracao.Buonny.IntegracaoBuonny.StatusMotorista(ref integracao, _tipoServicoMultisoftware, unidadeTrabalho);
                }
                catch (Exception excecao)
                {
                    Servicos.Log.TratarErro(excecao);
                    integracao.ProblemaIntegracao = Utilidades.String.Left(excecao.Message, 300);
                    integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                }

                repMotoristaIntegracao.Atualizar(integracao);
            }

            tipoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Buonny;
            List<Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao> listaVeiculosIntegracaoMotorista = repVeiculoIntegracao.BuscarPendentesIntegracaoPorTipo(numeroTentativas, minutosACadaTentativa, tipoIntegracao);
            for (var i = 0; i < listaVeiculosIntegracaoMotorista.Count; i++)
            {
                var integracao = listaVeiculosIntegracaoMotorista[i];
                integracao.DataIntegracao = DateTime.Now;
                integracao.NumeroTentativas += 1;

                try
                {
                    string mensagemErro = string.Empty;
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Buonny.RetornoStatusChecklist retorno = Servicos.Embarcador.Integracao.Buonny.IntegracaoBuonny.StatusChecklist(integracao.Veiculo.Placa, ref mensagemErro, unidadeTrabalho);
                    if (!string.IsNullOrWhiteSpace(mensagemErro))
                    {
                        integracao.ProblemaIntegracao = mensagemErro.Length > 300 ? mensagemErro.Substring(0, 300) : mensagemErro;
                        integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    }
                    else if (retorno == null)
                    {
                        integracao.ProblemaIntegracao = "Integração não teve retorno.";
                        integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    }
                    else if (!string.IsNullOrWhiteSpace(retorno.status) && retorno.status.ToUpper() == "S")
                    {
                        integracao.ProblemaIntegracao = "Checklist valido " + retorno.data_checklist;
                        integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    }
                    else
                    {
                        integracao.ProblemaIntegracao = "Veículo não possui checklist valido na Buonny.";
                        integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    }

                    string stringRetorno = string.Empty;
                    if (retorno != null)
                        stringRetorno = Newtonsoft.Json.JsonConvert.SerializeObject(retorno);

                    Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo integracaoArquivo = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                    integracaoArquivo.Mensagem = !string.IsNullOrWhiteSpace(stringRetorno) ? Utilidades.String.Left(stringRetorno, 400) : integracao.ProblemaIntegracao;
                    integracaoArquivo.Data = DateTime.Now;
                    integracaoArquivo.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento;
                    repCargaCTeIntegracaoArquivo.Inserir(integracaoArquivo);

                    if (integracao.ArquivosTransacao == null)
                        integracao.ArquivosTransacao = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();
                    integracao.ArquivosTransacao.Add(integracaoArquivo);
                }
                catch (Exception excecao)
                {
                    Servicos.Log.TratarErro(excecao);
                    integracao.ProblemaIntegracao = Utilidades.String.Left(excecao.Message, 300);
                    integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                }

                repVeiculoIntegracao.Atualizar(integracao);
            }

            tipoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.BuonnyRNTRC;
            List<Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao> listaVeiculosIntegracaoMotoristaRNTRC = repVeiculoIntegracao.BuscarPendentesIntegracaoPorTipo(numeroTentativas, minutosACadaTentativa, tipoIntegracao);
            for (var i = 0; i < listaVeiculosIntegracaoMotoristaRNTRC.Count; i++)
            {
                var integracao = listaVeiculosIntegracaoMotoristaRNTRC[i];
                integracao.DataIntegracao = DateTime.Now;
                integracao.NumeroTentativas += 1;

                try
                {
                    string mensagemErro = string.Empty;
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Buonny.RetornoStatusRNTRC retorno = Servicos.Embarcador.Integracao.Buonny.IntegracaoBuonny.StatusRNTRC(integracao.Veiculo.Placa, ref mensagemErro, unidadeTrabalho);
                    if (!string.IsNullOrWhiteSpace(mensagemErro))
                    {
                        integracao.ProblemaIntegracao = mensagemErro.Length > 300 ? mensagemErro.Substring(0, 300) : mensagemErro;
                        integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    }
                    else if (retorno == null)
                    {
                        integracao.ProblemaIntegracao = "Integração não teve retorno.";
                        integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    }
                    else if (!string.IsNullOrWhiteSpace(retorno.validado) && retorno.validado.ToUpper() == "S")
                    {
                        integracao.ProblemaIntegracao = "RNTRC valida.";
                        integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    }
                    else
                    {
                        integracao.ProblemaIntegracao = "Veículo não possui RNTRC valida na Buonny.";
                        integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    }

                    string stringRetorno = string.Empty;
                    if (retorno != null)
                        stringRetorno = Newtonsoft.Json.JsonConvert.SerializeObject(retorno);

                    Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo integracaoArquivo = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                    integracaoArquivo.Mensagem = !string.IsNullOrWhiteSpace(stringRetorno) ? Utilidades.String.Left(stringRetorno, 400) : integracao.ProblemaIntegracao;
                    integracaoArquivo.Data = DateTime.Now;
                    integracaoArquivo.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento;
                    repCargaCTeIntegracaoArquivo.Inserir(integracaoArquivo);

                    if (integracao.ArquivosTransacao == null)
                        integracao.ArquivosTransacao = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();
                    integracao.ArquivosTransacao.Add(integracaoArquivo);
                }
                catch (Exception excecao)
                {
                    Servicos.Log.TratarErro(excecao);
                    integracao.ProblemaIntegracao = Utilidades.String.Left(excecao.Message, 300);
                    integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                }

                repVeiculoIntegracao.Atualizar(integracao);
            }
        }
               
        #endregion Métodos Públicos

        #region Métodos Privados

        public void GerarIntegracaoTelerisco(Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Transportadores.MotoristaIntegracao repMotoristaIntegracao = new Repositorio.Embarcador.Transportadores.MotoristaIntegracao(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unidadeTrabalho);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeTrabalho);
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unidadeTrabalho);
            Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repIntegracao.Buscar();
            int numeroTentativas = 3;
            double minutosACadaTentativa = 5;

            var tipoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Telerisco;
            List<Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao> listaMotoristaIntegracao = repMotoristaIntegracao.BuscarPendentesIntegracaoPorTipo(numeroTentativas, minutosACadaTentativa, tipoIntegracao);

            for (var i = 0; i < listaMotoristaIntegracao.Count; i++)
            {
                var integracao = listaMotoristaIntegracao[i];
                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorMotorista(integracao.Motorista.Codigo);

                integracao.DataIntegracao = DateTime.Now;
                integracao.NumeroTentativas += 1;
                integracao.ProblemaIntegracao = string.Empty;
                integracao.Protocolo = string.Empty;
                integracao.Mensagem = string.Empty;
                integracao.DescricaoTipo = string.Empty;

                try
                {
                    string mensagemErro = string.Empty;
                    string jsonRequest = string.Empty;
                    string jsonResponse = string.Empty;

                    Dominio.ObjetosDeValor.Embarcador.Integracao.Telerisco.ConsultaMotoristaResponse retorno = Servicos.Embarcador.Integracao.Telerisco.IntegracaoTelerisco.ConsultaMotorista(integracao.Motorista, repFilial.BuscarMatriz(), DateTime.MinValue, ref mensagemErro, ref jsonRequest, ref jsonResponse, _tipoServicoMultisoftware, unidadeTrabalho, veiculo?.Placa);
                    if (!string.IsNullOrWhiteSpace(mensagemErro))
                    {
                        integracao.ProblemaIntegracao = mensagemErro.Length > 300 ? mensagemErro.Substring(0, 300) : mensagemErro;
                        integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    }
                    else if (retorno == null)
                    {
                        integracao.ProblemaIntegracao = "Integração não teve retorno.";
                        integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    }
                    else if (retorno.retornoWs == "1")
                    {
                        if (retorno.consulta == "1")
                        {
                            if (configuracaoIntegracao != null && !string.IsNullOrWhiteSpace(configuracaoIntegracao.CodigosAceitosRetornoTelerisco))
                            {
                                if (configuracaoIntegracao.CodigosAceitosRetornoTelerisco.Contains(retorno.categoriaResultado))
                                {
                                    integracao.ProblemaIntegracao = !string.IsNullOrWhiteSpace(retorno.consultaMensagem) ? retorno.consultaMensagem : "Consulta retornada com sucesso";
                                    integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                                    integracao.Protocolo = retorno.protocolo;
                                    integracao.Mensagem = retorno.consultaMensagem;
                                    integracao.DescricaoTipo = retorno.tipoMotorista?.ToString() ?? retorno.mensagemRetorno;
                                }
                                else
                                {
                                    integracao.ProblemaIntegracao = !string.IsNullOrWhiteSpace(retorno.consultaMensagem) ? retorno.consultaMensagem : !string.IsNullOrWhiteSpace(retorno.resultado) ? retorno.resultado : "Consulta sem mensagem de retorno";
                                    integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                                }
                            }
                            else if (retorno.categoriaResultado == "350" || retorno.categoriaResultado == "250" || retorno.categoriaResultado == "500" || retorno.categoriaResultado == "400" || retorno.categoriaResultado == "280" || retorno.categoriaResultado == "200" || retorno.categoriaResultado == "100")
                            {
                                integracao.ProblemaIntegracao = !string.IsNullOrWhiteSpace(retorno.consultaMensagem) ? retorno.consultaMensagem : "Consulta retornada com sucesso";
                                integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                                integracao.Protocolo = retorno.protocolo;
                                integracao.Mensagem = retorno.consultaMensagem;
                                integracao.DescricaoTipo = retorno.tipoMotorista.ToString();
                            }
                            else
                            {
                                integracao.ProblemaIntegracao = !string.IsNullOrWhiteSpace(retorno.consultaMensagem) ? retorno.consultaMensagem : "Consulta sem mensagem de retorno";
                                integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                            }
                        }
                        else if (retorno.consulta == "2")
                        {
                            integracao.ProblemaIntegracao = !string.IsNullOrWhiteSpace(retorno.consultaMensagem) ? retorno.consultaMensagem : "Consulta não Localizada";
                            integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                        }
                        else if (retorno.consulta == "3")
                        {
                            integracao.ProblemaIntegracao = !string.IsNullOrWhiteSpace(retorno.consultaMensagem) ? retorno.consultaMensagem : "Perfil atual do motorista diferente da última consulta, necessário nova consulta";
                            integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                        }
                        else if (retorno.consulta == "4")
                        {
                            integracao.ProblemaIntegracao = !string.IsNullOrWhiteSpace(retorno.consultaMensagem) ? retorno.consultaMensagem : "Motorista com restrição com a empresa Transportador/Embarcador";
                            integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                        }
                        else
                        {
                            integracao.ProblemaIntegracao = !string.IsNullOrWhiteSpace(retorno.consultaMensagem) ? retorno.consulta + " - " + retorno.consultaMensagem : "Código consulta " + retorno.consulta + " não previsto no manual da Telerisco";
                            integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                        }
                    }
                    else
                    {
                        integracao.ProblemaIntegracao = retorno.mensagemRetorno;
                        integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    }

                    string stringRetorno = string.Empty;
                    if (retorno != null)
                        stringRetorno = Newtonsoft.Json.JsonConvert.SerializeObject(retorno);

                    Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo integracaoArquivo = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                    integracaoArquivo.Mensagem = !string.IsNullOrWhiteSpace(stringRetorno) ? Utilidades.String.Left(stringRetorno, 400) : integracao.ProblemaIntegracao;
                    integracaoArquivo.Data = DateTime.Now;
                    integracaoArquivo.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento;

                    integracaoArquivo.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequest, "json", unidadeTrabalho);
                    integracaoArquivo.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResponse, "json", unidadeTrabalho);

                    repCargaCTeIntegracaoArquivo.Inserir(integracaoArquivo);

                    if (integracao.ArquivosTransacao == null)
                        integracao.ArquivosTransacao = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();

                    integracao.ArquivosTransacao.Add(integracaoArquivo);
                }
                catch (Exception excecao)
                {
                    Servicos.Log.TratarErro(excecao);
                    integracao.ProblemaIntegracao = Utilidades.String.Left(excecao.Message, 300);
                    integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                }

                repMotoristaIntegracao.Atualizar(integracao);
            }
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private void GerarIntegracaoUltragaz(Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Veiculos.VeiculoIntegracao repVeiculoIntegracao = new Repositorio.Embarcador.Veiculos.VeiculoIntegracao(unidadeTrabalho);
            Servicos.Embarcador.Integracao.Ultragaz.IntegracaoUltragaz servicoUltragaz = new Servicos.Embarcador.Integracao.Ultragaz.IntegracaoUltragaz(unidadeTrabalho, _tipoServicoMultisoftware);

            List<Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao> listaVeiculoIntegracao = repVeiculoIntegracao.BuscarPendentesIntegracaoPorTipo(3, 5, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Ultragaz);

            foreach (Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao integracao in listaVeiculoIntegracao)
            {
                servicoUltragaz.IntegrarVeiculo(integracao);
            }
        }

        private void GerarIntegracaoFrota162(Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Veiculos.VeiculoIntegracao repVeiculoIntegracao = new Repositorio.Embarcador.Veiculos.VeiculoIntegracao(unidadeTrabalho);
            Repositorio.Embarcador.Transportadores.MotoristaIntegracao repMotoristaIntegracao = new Repositorio.Embarcador.Transportadores.MotoristaIntegracao(unidadeTrabalho);

            Servicos.Embarcador.Integracao.Frota162.IntegracaoFrota162 servicoIntegracaoFrota162 = new Servicos.Embarcador.Integracao.Frota162.IntegracaoFrota162(unidadeTrabalho);

            int numeroTentativas = 3;
            double minutosACadaTentativa = 5;

            List<Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao> listaMotoristaIntegracao = repMotoristaIntegracao.BuscarPendentesIntegracaoPorTipo(numeroTentativas, minutosACadaTentativa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Frota162);
            foreach (Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao motoristaIntegracao in listaMotoristaIntegracao)
                servicoIntegracaoFrota162.IntegrarMotorista(motoristaIntegracao);

            List<Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao> listaVeiculoIntegracao = repVeiculoIntegracao.BuscarPendentesIntegracaoPorTipo(numeroTentativas, minutosACadaTentativa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Frota162);
            foreach (Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao veiculoIntegracao in listaVeiculoIntegracao)
            {
                if (veiculoIntegracao.Veiculo.Status == "I")
                    servicoIntegracaoFrota162.InativarVeiculo(veiculoIntegracao);
                else
                    servicoIntegracaoFrota162.IntegrarVeiculo(veiculoIntegracao);
            }
        }

        private void GerarIntegracaoKMM(Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Veiculos.VeiculoIntegracao repVeiculoIntegracao = new Repositorio.Embarcador.Veiculos.VeiculoIntegracao(unidadeTrabalho);
            Repositorio.Embarcador.Transportadores.MotoristaIntegracao repMotoristaIntegracao = new Repositorio.Embarcador.Transportadores.MotoristaIntegracao(unidadeTrabalho);

            Servicos.Embarcador.Integracao.KMM.IntegracaoKMM servicoIntegracaoKMM = new Servicos.Embarcador.Integracao.KMM.IntegracaoKMM(unidadeTrabalho);

            int numeroTentativas = 3;
            double minutosACadaTentativa = 5;

            List<Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao> listaMotoristaIntegracao = repMotoristaIntegracao.BuscarPendentesIntegracaoPorTipo(numeroTentativas, minutosACadaTentativa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.KMM);
            foreach (Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao motoristaIntegracao in listaMotoristaIntegracao)
                servicoIntegracaoKMM.IntegrarPessoaMotorista(motoristaIntegracao);

            List<Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao> listaVeiculoIntegracao = repVeiculoIntegracao.BuscarPendentesIntegracaoPorTipo(numeroTentativas, minutosACadaTentativa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.KMM);
            foreach (Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao veiculoIntegracao in listaVeiculoIntegracao)
                servicoIntegracaoKMM.IntegrarVeiculo(veiculoIntegracao);
        }

        private void GerarIntegracaoMotoristaBrasilRisk(Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Transportadores.MotoristaIntegracao repositorioMotoristaIntegracao = new Repositorio.Embarcador.Transportadores.MotoristaIntegracao(unidadeTrabalho);

            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tiposIntegracao = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> { Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.BrasilRiskGestao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.BrasilRiskVeiculoMotorista };

            List<Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao> integracoesPendentes = repositorioMotoristaIntegracao.BuscarPendentesIntegracaoPorTipos(3, 5, tiposIntegracao);

            foreach (Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao integracaoPendente in integracoesPendentes)
            {
                switch (integracaoPendente.TipoIntegracao.Tipo)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.BrasilRiskGestao:
                        IntegrarConsultaMotoristaBrasilRisk(integracaoPendente, unidadeTrabalho);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.BrasilRiskVeiculoMotorista:
                        new Servicos.Embarcador.Integracao.BrasilRisk.IntegracaoBrasilRisk(unidadeTrabalho).CadastrarMotoristaAnalisePerfil(integracaoPendente);
                        break;
                    default:
                        integracaoPendente.NumeroTentativas++;
                        integracaoPendente.DataIntegracao = DateTime.Now;
                        integracaoPendente.ProblemaIntegracao = "Integração não configurada para processar";
                        integracaoPendente.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                        repositorioMotoristaIntegracao.Atualizar(integracaoPendente);
                        break;
                }
            }
        }

        public void IntegrarMotoristaBrasilRiskAguardandoRetorno(Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Transportadores.MotoristaIntegracao repositorioMotoristaIntegracao = new Repositorio.Embarcador.Transportadores.MotoristaIntegracao(unidadeTrabalho);

            int numeroTentativas = 18;
            double minutosACadaTentativa = 20;

            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tiposIntegracao = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> { Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.BrasilRiskVeiculoMotorista };

            List<Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao> integracoesPendentes = repositorioMotoristaIntegracao.BuscarIntegracaoAguardandoRetornoPorTiposEDataHora(numeroTentativas, minutosACadaTentativa, tiposIntegracao);

            foreach (Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao integracaoPendente in integracoesPendentes)
            {
                switch (integracaoPendente.TipoIntegracao.Tipo)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.BrasilRiskVeiculoMotorista:
                        new Servicos.Embarcador.Integracao.BrasilRisk.IntegracaoBrasilRisk(unidadeTrabalho).ConstultarStatusMotoristaAnalisePerfil(integracaoPendente);
                        break;
                    default:
                        integracaoPendente.NumeroTentativas++;
                        integracaoPendente.DataIntegracao = DateTime.Now;
                        integracaoPendente.ProblemaIntegracao = "Integração não configurada para processar";
                        integracaoPendente.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                        repositorioMotoristaIntegracao.Atualizar(integracaoPendente);
                        break;
                }
            }
        }

        public void IntegrarVeiculoBrasilRiskAguardandoRetorno(Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Veiculos.VeiculoIntegracao repositorioVeiculoIntegracao = new Repositorio.Embarcador.Veiculos.VeiculoIntegracao(unidadeTrabalho);

            int numeroTentativas = 18;
            double minutosACadaTentativa = 20;

            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tiposIntegracao = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> { Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.BrasilRiskVeiculoMotorista };

            List<Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao> integracoesPendentes = repositorioVeiculoIntegracao.BuscarIntegracaoAguardandoRetornoPorTiposEDataHora(numeroTentativas, minutosACadaTentativa, tiposIntegracao);

            foreach (Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao integracaoPendente in integracoesPendentes)
            {
                switch (integracaoPendente.TipoIntegracao.Tipo)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.BrasilRiskVeiculoMotorista:
                        new Servicos.Embarcador.Integracao.BrasilRisk.IntegracaoBrasilRisk(unidadeTrabalho).ConsultarStatusVeiculoAnalisePerfil(integracaoPendente);
                        break;
                    default:
                        integracaoPendente.NumeroTentativas++;
                        integracaoPendente.DataIntegracao = DateTime.Now;
                        integracaoPendente.ProblemaIntegracao = "Integração não configurada para processar";
                        integracaoPendente.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                        repositorioVeiculoIntegracao.Atualizar(integracaoPendente);

                        break;
                }
            }
        }

        private void GerarIntegracaoVeiculoBrasilRisk(Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Veiculos.VeiculoIntegracao repositorioVeiculoIntegracao = new Repositorio.Embarcador.Veiculos.VeiculoIntegracao(unidadeTrabalho);

            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tiposIntegracao = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> { Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.BrasilRiskGestao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.BrasilRiskVeiculoMotorista };

            List<Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao> integracoesPendentes = repositorioVeiculoIntegracao.BuscarPendentesIntegracaoPorTipos(3, 5, tiposIntegracao);

            foreach (Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao integracaoPendente in integracoesPendentes)
            {
                switch (integracaoPendente.TipoIntegracao.Tipo)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.BrasilRiskGestao:
                        IntegrarConsultaVeiculoBrasilRisk(integracaoPendente, unidadeTrabalho);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.BrasilRiskVeiculoMotorista:
                        new Servicos.Embarcador.Integracao.BrasilRisk.IntegracaoBrasilRisk(unidadeTrabalho).CadastrarVeiculoAnalisePerfil(integracaoPendente);
                        break;
                    default:
                        integracaoPendente.NumeroTentativas++;
                        integracaoPendente.DataIntegracao = DateTime.Now;
                        integracaoPendente.ProblemaIntegracao = "Integração não configurada para processar";
                        integracaoPendente.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                        repositorioVeiculoIntegracao.Atualizar(integracaoPendente);

                        break;
                }
            }
        }

        private void IntegrarConsultaMotoristaBrasilRisk(Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao integracao, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Transportadores.MotoristaIntegracao repositorioMotoristaIntegracao = new Repositorio.Embarcador.Transportadores.MotoristaIntegracao(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repositorioCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unidadeTrabalho);

            integracao.DataIntegracao = DateTime.Now;
            integracao.NumeroTentativas += 1;
            integracao.ProblemaIntegracao = string.Empty;
            integracao.Protocolo = string.Empty;
            integracao.Mensagem = string.Empty;
            integracao.DescricaoTipo = string.Empty;

            try
            {
                string mensagemErro = string.Empty;
                string xmlRequest = string.Empty;
                string xmlResponse = string.Empty;

                Servicos.ServicoBrasilRisk.GestaoAnaliseDePerfil.RetornoConsulta retorno = Servicos.Embarcador.Integracao.BrasilRisk.IntegracaoBrasilRisk.ConsultaMotorista(integracao.Motorista.CPF, ref mensagemErro, ref xmlRequest, ref xmlResponse, unidadeTrabalho);

                if (!string.IsNullOrWhiteSpace(mensagemErro))
                {
                    integracao.ProblemaIntegracao = mensagemErro.Length > 300 ? mensagemErro.Substring(0, 300) : mensagemErro;
                    integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                }
                else if (retorno == null)
                {
                    integracao.ProblemaIntegracao = "Integração não teve retorno.";
                    integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                }
                else if (retorno.Status)
                {
                    integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    integracao.Mensagem = retorno.Mensagem;
                }
                else
                {
                    integracao.ProblemaIntegracao = retorno.Mensagem;
                    integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                }

                string stringRetorno = string.Empty;
                if (retorno != null)
                    stringRetorno = Newtonsoft.Json.JsonConvert.SerializeObject(retorno);

                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo integracaoArquivo = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                integracaoArquivo.Mensagem = !string.IsNullOrWhiteSpace(stringRetorno) ? Utilidades.String.Left(stringRetorno, 400) : integracao.ProblemaIntegracao;
                integracaoArquivo.Data = DateTime.Now;
                integracaoArquivo.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento;

                integracaoArquivo.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlRequest, "xml", unidadeTrabalho);
                integracaoArquivo.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlResponse, "xml", unidadeTrabalho);

                repositorioCargaCTeIntegracaoArquivo.Inserir(integracaoArquivo);

                if (integracao.ArquivosTransacao == null)
                    integracao.ArquivosTransacao = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();

                integracao.ArquivosTransacao.Add(integracaoArquivo);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                integracao.ProblemaIntegracao = Utilidades.String.Left(excecao.Message, 300);
                integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }

            repositorioMotoristaIntegracao.Atualizar(integracao);
        }

        private void IntegrarConsultaVeiculoBrasilRisk(Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao integracao, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Veiculos.VeiculoIntegracao repositorioVeiculoIntegracao = new Repositorio.Embarcador.Veiculos.VeiculoIntegracao(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repositorioCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unidadeTrabalho);

            integracao.DataIntegracao = DateTime.Now;
            integracao.NumeroTentativas += 1;
            integracao.ProblemaIntegracao = string.Empty;

            try
            {
                string mensagemErro = string.Empty;
                string xmlRequest = string.Empty;
                string xmlResponse = string.Empty;

                Servicos.ServicoBrasilRisk.GestaoAnaliseDePerfil.RetornoConsulta retorno = Servicos.Embarcador.Integracao.BrasilRisk.IntegracaoBrasilRisk.ConsultaVeiculo(integracao.Veiculo.Placa, ref mensagemErro, ref xmlRequest, ref xmlResponse, unidadeTrabalho);

                if (!string.IsNullOrWhiteSpace(mensagemErro))
                {
                    integracao.ProblemaIntegracao = mensagemErro.Length > 300 ? mensagemErro.Substring(0, 300) : mensagemErro;
                    integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                }
                else if (retorno == null)
                {
                    integracao.ProblemaIntegracao = "Integração não teve retorno.";
                    integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                }
                else if (retorno.Status)
                {
                    integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                }
                else
                {
                    integracao.ProblemaIntegracao = retorno.Mensagem;
                    integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                }

                string stringRetorno = string.Empty;

                if (retorno != null)
                    stringRetorno = Newtonsoft.Json.JsonConvert.SerializeObject(retorno);

                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo integracaoArquivo = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                integracaoArquivo.Mensagem = !string.IsNullOrWhiteSpace(stringRetorno) ? Utilidades.String.Left(stringRetorno, 400) : integracao.ProblemaIntegracao;
                integracaoArquivo.Data = DateTime.Now;
                integracaoArquivo.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento;

                integracaoArquivo.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlRequest, "xml", unidadeTrabalho);
                integracaoArquivo.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlResponse, "xml", unidadeTrabalho);

                repositorioCargaCTeIntegracaoArquivo.Inserir(integracaoArquivo);

                if (integracao.ArquivosTransacao == null)
                    integracao.ArquivosTransacao = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();

                integracao.ArquivosTransacao.Add(integracaoArquivo);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                integracao.ProblemaIntegracao = Utilidades.String.Left(excecao.Message, 300);
                integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }

            repositorioVeiculoIntegracao.Atualizar(integracao);
        }

        #endregion Métodos Privados
    }
}