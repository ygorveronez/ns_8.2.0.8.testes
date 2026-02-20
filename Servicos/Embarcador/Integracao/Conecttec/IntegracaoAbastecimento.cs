using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Newtonsoft.Json;
using System;
using Dominio.ObjetosDeValor.Embarcador.Integracao.Conecttec;
using Dominio.Excecoes.Embarcador;
using Dominio.Entidades.Embarcador.Integracao;
using Dominio.ObjetosDeValor.WebService.Abastecimento;
using AdminMultisoftware.Dominio.Enumeradores;
using System.Collections.Generic;

namespace Servicos.Embarcador.Integracao.Conecttec
{
    public partial class IntegracaoConecttec
    {
        #region Métodos Globais
        public void IntegrarReservaAbastecimento(Dominio.Entidades.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizadoIntegracao liberacaoAbastecimentoAutomatizadoIntegracao)
        {
            
            Repositorio.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizadoIntegracao repLiberacaoAbastecimentoAutomatizadoIntegracao = new Repositorio.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizadoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repArquivoIntegracao = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(_unitOfWork);
            Repositorio.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizado repLiberacaoAbastecimentoAutomatizado = new Repositorio.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizado(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            string jsonRequisicao = "";
            string jsonRetorno = "";

            try
            {
                liberacaoAbastecimentoAutomatizadoIntegracao.NumeroTentativas += 1;
                liberacaoAbastecimentoAutomatizadoIntegracao.DataIntegracao = DateTime.Now;

                object request = this.ObterReserva(liberacaoAbastecimentoAutomatizadoIntegracao);
                Dominio.ObjetosDeValor.Embarcador.Integracao.Conecttec.retornoWebService retWS = Transmitir("V4/Delivery/Reserve", request);

                if (retWS.SituacaoIntegracao == SituacaoIntegracao.Integrado)
                {
                    var retornoWS = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Conecttec.StationReservation>(retWS.jsonRetorno);
                    if (!string.IsNullOrEmpty(retornoWS.ReserveId))
                    {
                        liberacaoAbastecimentoAutomatizadoIntegracao.ReserveID = retornoWS.ReserveId;
                        liberacaoAbastecimentoAutomatizadoIntegracao.LiberacaoAbastecimentoAutomatizado.SituacaoAbastecimento = SituacaoIntegracaoAbastecimento.AgRetornoReserva;

                    }
                    else
                    {
                        throw new ServicoException($"Integração Conecttec não retornou o ReserveID");
                    }
                }
                else
                {
                    liberacaoAbastecimentoAutomatizadoIntegracao.ProblemaIntegracao = retWS.ProblemaIntegracao;
                    liberacaoAbastecimentoAutomatizadoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    liberacaoAbastecimentoAutomatizadoIntegracao.LiberacaoAbastecimentoAutomatizado.SituacaoAbastecimento = SituacaoIntegracaoAbastecimento.ProblemaAbastecimento;

                }

                jsonRequisicao = retWS.jsonRequisicao;
                jsonRetorno = retWS.jsonRetorno;

                repLiberacaoAbastecimentoAutomatizado.Atualizar(liberacaoAbastecimentoAutomatizadoIntegracao.LiberacaoAbastecimentoAutomatizado);

            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                liberacaoAbastecimentoAutomatizadoIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a Conecttec";
                liberacaoAbastecimentoAutomatizadoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }

            servicoArquivoTransacao.Adicionar(liberacaoAbastecimentoAutomatizadoIntegracao, jsonRequisicao, jsonRetorno, "json");

            repLiberacaoAbastecimentoAutomatizadoIntegracao.Atualizar(liberacaoAbastecimentoAutomatizadoIntegracao);
        }

        public void IntegrarAutorizacaoAbastecimento(Dominio.Entidades.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizadoIntegracao liberacaoAbastecimentoAutomatizadoIntegracao)
        {
            Repositorio.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizadoIntegracao repLiberacaoAbastecimentoAutomatizadoIntegracao = new Repositorio.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizadoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repArquivoIntegracao = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(_unitOfWork);
            Repositorio.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizado repLiberacaoAbastecimentoAutomatizado = new Repositorio.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizado(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            string jsonRequisicao = "";
            string jsonRetorno = "";

            try
            {
                liberacaoAbastecimentoAutomatizadoIntegracao.NumeroTentativas += 1;
                liberacaoAbastecimentoAutomatizadoIntegracao.DataIntegracao = DateTime.Now;


                object request = this.ObterAuthorize(liberacaoAbastecimentoAutomatizadoIntegracao);
                Dominio.ObjetosDeValor.Embarcador.Integracao.Conecttec.retornoWebService retWS = Transmitir("Delivery/Authorize", request);

                if (retWS.SituacaoIntegracao == SituacaoIntegracao.Integrado)
                {
                    var retornoWS = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Conecttec.authorizeReturn>(retWS.jsonRetorno);
                    if (!string.IsNullOrEmpty(retornoWS.AuthId))
                    {
                        liberacaoAbastecimentoAutomatizadoIntegracao.LiberacaoAbastecimentoAutomatizado.SituacaoAbastecimento = SituacaoIntegracaoAbastecimento.AgRetornoAutorizacao;
                        liberacaoAbastecimentoAutomatizadoIntegracao.DataAutorizacao = DateTime.Now;
                        liberacaoAbastecimentoAutomatizadoIntegracao.AuthID = retornoWS.AuthId;
                    }
                    else
                    {
                        throw new ServicoException($"Integração Conecttec não autorizou o abastecimento");
                    }
                }
                else
                {
                    liberacaoAbastecimentoAutomatizadoIntegracao.ProblemaIntegracao = retWS.ProblemaIntegracao;
                    liberacaoAbastecimentoAutomatizadoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    liberacaoAbastecimentoAutomatizadoIntegracao.LiberacaoAbastecimentoAutomatizado.SituacaoAbastecimento = SituacaoIntegracaoAbastecimento.ProblemaAbastecimento;
                }

                jsonRequisicao = retWS.jsonRequisicao;
                jsonRetorno = retWS.jsonRetorno;

                repLiberacaoAbastecimentoAutomatizado.Atualizar(liberacaoAbastecimentoAutomatizadoIntegracao.LiberacaoAbastecimentoAutomatizado);

            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                liberacaoAbastecimentoAutomatizadoIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a Conecttec";
                liberacaoAbastecimentoAutomatizadoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }

            servicoArquivoTransacao.Adicionar(liberacaoAbastecimentoAutomatizadoIntegracao, jsonRequisicao, jsonRetorno, "json");

            repLiberacaoAbastecimentoAutomatizadoIntegracao.Atualizar(liberacaoAbastecimentoAutomatizadoIntegracao);
        }

        public Dominio.ObjetosDeValor.Embarcador.Integracao.Conecttec.deliveryReserve ObterReserva(Dominio.Entidades.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizadoIntegracao liberacaoAbastecimentoAutomatizadoIntegracao)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Conecttec.deliveryReserve retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Conecttec.deliveryReserve();

            int.TryParse(liberacaoAbastecimentoAutomatizadoIntegracao.LiberacaoAbastecimentoAutomatizado?.BombaAbastecimento?.CodigoBombaIntegracao, out int codigoBombaIntegracao);

            retorno.StationId = Convert.ToInt32(_configuracaoIntegracaoConecttec.StationID);
            retorno.FuelPointNumber = codigoBombaIntegracao;
            retorno.ProviderId = _configuracaoIntegracaoConecttec.ProviderID;
            retorno.ReserveTimeOut = 40;

            return retorno;
        }
        public Dominio.ObjetosDeValor.Embarcador.Integracao.Conecttec.deliveryAuthorize ObterAuthorize(Dominio.Entidades.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizadoIntegracao liberacaoAbastecimentoAutomatizadoIntegracao)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Conecttec.deliveryAuthorize retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Conecttec.deliveryAuthorize();

            if(string.IsNullOrEmpty(liberacaoAbastecimentoAutomatizadoIntegracao.ReserveID))
                throw new ServicoException($"Não possui reserveID");


            int.TryParse(liberacaoAbastecimentoAutomatizadoIntegracao.LiberacaoAbastecimentoAutomatizado?.BombaAbastecimento?.CodigoBicoIntegracao, out int codigoBicoIntegracao);

            retorno.ReserveId = liberacaoAbastecimentoAutomatizadoIntegracao.ReserveID;
            retorno.HoseNumber = codigoBicoIntegracao;
            retorno.LimitType = "V";
            retorno.LimitValue = 999;
            retorno.Price = Convert.ToDouble(liberacaoAbastecimentoAutomatizadoIntegracao.LiberacaoAbastecimentoAutomatizado?.BombaAbastecimento?.LocalArmazenamentoProduto?.TipoOleo?.Produto?.UltimoCusto ?? 0);
            retorno.UpdateRate = 2;
            retorno.AuthTimeout = 80;

            return retorno;
        }


        public bool AtualizarStatusAbastecimento(Dominio.ObjetosDeValor.WebService.Abastecimento.AtualizarStatusAbastecimentoConecttec atualizarStatusAbastecimentoConecttec)
        {
            try
            {
                this.RegistrarRequisicao(atualizarStatusAbastecimentoConecttec, atualizarStatusAbastecimentoConecttec?.Fuel?.ReserveId ?? atualizarStatusAbastecimentoConecttec?.ReserveId ?? "");

                if (!(_configuracaoIntegracaoConecttec?.PossuiIntegracao ?? false))
                    throw new Exception("AtualizarStatusAbastecimento: Não possui configuração para Conecttec.");

                if (atualizarStatusAbastecimentoConecttec.Fuel == null)
                    throw new Exception("AtualizarStatusAbastecimento: Não retornou a tag Fuel");

                Repositorio.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizadoIntegracao repLiberacaoAbastecimentoAutomatizadoIntegracao = new Repositorio.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizadoIntegracao(_unitOfWork);
                Repositorio.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizado repLiberacaoAbastecimentoAutomatizado = new Repositorio.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizado(_unitOfWork);

                Dominio.Entidades.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizadoIntegracao liberacaoAbastecimentoAutomatizadoIntegracao = null;
                Dominio.Entidades.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizado liberacaoAbastecimentoAutomatizado = null;

                liberacaoAbastecimentoAutomatizadoIntegracao = repLiberacaoAbastecimentoAutomatizadoIntegracao.BuscarPorReserveID(atualizarStatusAbastecimentoConecttec.Fuel?.ReserveId ?? atualizarStatusAbastecimentoConecttec.ReserveId ?? "");
                liberacaoAbastecimentoAutomatizado = repLiberacaoAbastecimentoAutomatizado.BuscarPorCodigo(liberacaoAbastecimentoAutomatizadoIntegracao?.LiberacaoAbastecimentoAutomatizado?.Codigo ?? 0, false);

                if (liberacaoAbastecimentoAutomatizadoIntegracao == null || liberacaoAbastecimentoAutomatizado == null)
                    throw new Exception("AtualizarStatusAbastecimento: Não encontrou o registro através do reserve ID");

                int reason = atualizarStatusAbastecimentoConecttec.Fuel?.Reason ?? atualizarStatusAbastecimentoConecttec.Reason ?? 0;
                int fuelpointstatus = atualizarStatusAbastecimentoConecttec.Fuel?.FuelPointStatus ?? atualizarStatusAbastecimentoConecttec.FuelPointStatus ?? 0;

                if (liberacaoAbastecimentoAutomatizado.SituacaoAbastecimento == SituacaoIntegracaoAbastecimento.AgRetornoReserva)
                {
                    if (reason == (int)SituacaoAbastecimentoConecttec.RESERVE_SUCCESSFUL && (fuelpointstatus == 1 || fuelpointstatus == 2))
                    {
                        liberacaoAbastecimentoAutomatizadoIntegracao.LiberacaoAbastecimentoAutomatizado.SituacaoAbastecimento = SituacaoIntegracaoAbastecimento.PendenteAutorizacao;

                    }
                    else
                    {
                        liberacaoAbastecimentoAutomatizadoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                        liberacaoAbastecimentoAutomatizadoIntegracao.LiberacaoAbastecimentoAutomatizado.SituacaoAbastecimento = SituacaoIntegracaoAbastecimento.ProblemaAbastecimento;
                        liberacaoAbastecimentoAutomatizadoIntegracao.ProblemaIntegracao = (((SituacaoAbastecimentoConecttec?)reason ?? SituacaoAbastecimentoConecttec.AUTH_FAILED).ObterDescricao() ?? "") + " | FuelPointStatus = " + fuelpointstatus;

                    }
                }
                else if (liberacaoAbastecimentoAutomatizado.SituacaoAbastecimento == SituacaoIntegracaoAbastecimento.AgRetornoAutorizacao)
                {
                    if (reason != (int)SituacaoAbastecimentoConecttec.RESERVE_SUCCESSFUL)
                    {
                        if (reason == (int)SituacaoAbastecimentoConecttec.AUTH_SUCESSFUL && (fuelpointstatus == 1 || fuelpointstatus == 2 || fuelpointstatus == 3))
                        {
                            liberacaoAbastecimentoAutomatizadoIntegracao.LiberacaoAbastecimentoAutomatizado.SituacaoAbastecimento = SituacaoIntegracaoAbastecimento.Autorizado;

                        }
                        else
                        {
                            liberacaoAbastecimentoAutomatizadoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                            liberacaoAbastecimentoAutomatizadoIntegracao.LiberacaoAbastecimentoAutomatizado.SituacaoAbastecimento = SituacaoIntegracaoAbastecimento.ProblemaAbastecimento;
                            liberacaoAbastecimentoAutomatizadoIntegracao.ProblemaIntegracao = (((SituacaoAbastecimentoConecttec?)reason ?? SituacaoAbastecimentoConecttec.AUTH_FAILED).ObterDescricao() ?? "") + " | FuelPointStatus = " + fuelpointstatus;

                        }
                    }
                }
                else if (liberacaoAbastecimentoAutomatizado.SituacaoAbastecimento == SituacaoIntegracaoAbastecimento.Autorizado)
                {
                    if (reason != (int)SituacaoAbastecimentoConecttec.AUTH_SUCESSFUL)
                    {
                        if (reason == (int)SituacaoAbastecimentoConecttec.RUNNING_TOTAL && fuelpointstatus == 4)
                        {
                            liberacaoAbastecimentoAutomatizadoIntegracao.LiberacaoAbastecimentoAutomatizado.SituacaoAbastecimento = SituacaoIntegracaoAbastecimento.AgRetornoAbastecimento;

                        }
                        else
                        {
                            liberacaoAbastecimentoAutomatizadoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                            liberacaoAbastecimentoAutomatizadoIntegracao.LiberacaoAbastecimentoAutomatizado.SituacaoAbastecimento = SituacaoIntegracaoAbastecimento.ProblemaAbastecimento;
                            liberacaoAbastecimentoAutomatizadoIntegracao.ProblemaIntegracao = (((SituacaoAbastecimentoConecttec?)reason ?? SituacaoAbastecimentoConecttec.AUTH_FAILED).ObterDescricao() ?? "") + " | FuelPointStatus = " + fuelpointstatus;

                        }
                    }
                }
                else
                {
                    throw new Exception(@"AtualizarStatusAbastecimento: Situação do abastecimento diferente do esperado: " + liberacaoAbastecimentoAutomatizado.SituacaoAbastecimento.ObterDescricao() + " | Motivo: " + liberacaoAbastecimentoAutomatizadoIntegracao.ProblemaIntegracao ?? "");
                }

                repLiberacaoAbastecimentoAutomatizadoIntegracao.Atualizar(liberacaoAbastecimentoAutomatizadoIntegracao);
                repLiberacaoAbastecimentoAutomatizado.Atualizar(liberacaoAbastecimentoAutomatizado);

                return true;
                    
            }
            catch (ServicoException excecao)
            {
                Log.TratarErro(excecao);

                return false;
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);

                return false;
            }

        }

        public bool FinalizarAbastecimento(Dominio.ObjetosDeValor.WebService.Abastecimento.FinalizarAbastecimentoConecttec finalizarAbastecimentoConecttec)
        {
            try
            {
                this.RegistrarRequisicao(finalizarAbastecimentoConecttec, finalizarAbastecimentoConecttec?.ReserveId ?? "");

                if (!(_configuracaoIntegracaoConecttec?.PossuiIntegracao ?? false))
                    throw new Exception("FinalizarAbastecimento: Não possui configuração para Conecttec.");

                if (string.IsNullOrEmpty(finalizarAbastecimentoConecttec.ReserveId) || string.IsNullOrEmpty(finalizarAbastecimentoConecttec.AuthId))
                    throw new Exception("FinalizarAbastecimento: Não retornou a tag ReserveId/AuthId");

                Repositorio.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizadoIntegracao repLiberacaoAbastecimentoAutomatizadoIntegracao = new Repositorio.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizadoIntegracao(_unitOfWork);
                Repositorio.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizado repLiberacaoAbastecimentoAutomatizado = new Repositorio.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizado(_unitOfWork);

                Dominio.Entidades.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizadoIntegracao liberacaoAbastecimentoAutomatizadoIntegracao = null;
                Dominio.Entidades.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizado liberacaoAbastecimentoAutomatizado = null;

                liberacaoAbastecimentoAutomatizadoIntegracao = repLiberacaoAbastecimentoAutomatizadoIntegracao.BuscarPorReserveIDEAuthID(finalizarAbastecimentoConecttec.ReserveId ?? "", finalizarAbastecimentoConecttec.AuthId ?? "");
                liberacaoAbastecimentoAutomatizado = repLiberacaoAbastecimentoAutomatizado.BuscarPorCodigo(liberacaoAbastecimentoAutomatizadoIntegracao?.LiberacaoAbastecimentoAutomatizado?.Codigo ?? 0 , false);

                if (liberacaoAbastecimentoAutomatizadoIntegracao == null || liberacaoAbastecimentoAutomatizado == null)
                    throw new Exception("AtualizarStatusAbastecimento: Não encontrou o registro através do reserve ID ("+ finalizarAbastecimentoConecttec.ReserveId + ") e Auth ID ("+ finalizarAbastecimentoConecttec.AuthId + ") ");


                if (liberacaoAbastecimentoAutomatizado.SituacaoAbastecimento == SituacaoIntegracaoAbastecimento.AgRetornoAbastecimento)
                {

                    string mensagemErro = string.Empty;
                    int codigoFechamento = 0;

                    liberacaoAbastecimentoAutomatizadoIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    liberacaoAbastecimentoAutomatizadoIntegracao.LiberacaoAbastecimentoAutomatizado.SituacaoAbastecimento = SituacaoIntegracaoAbastecimento.Finalizado;
                    liberacaoAbastecimentoAutomatizadoIntegracao.ProblemaIntegracao = "Abastecimento concluído";
                    liberacaoAbastecimentoAutomatizadoIntegracao.QuantidadeAbastecida = finalizarAbastecimentoConecttec.Volume ?? 0;

                    if (!this.CriarRegistroAbastecimento(finalizarAbastecimentoConecttec, liberacaoAbastecimentoAutomatizado, liberacaoAbastecimentoAutomatizadoIntegracao))
                        throw new Exception(@"CriarRegistroAbastecimento: Erro ao criar o registro do abastecimento.");

                    if (!this.GerarFechamentoAbastecimento(ref mensagemErro, ref codigoFechamento, liberacaoAbastecimentoAutomatizado))
                        throw new Exception(@$"GerarFechamentoAbastecimento: {mensagemErro}.");

                    if (!this.FecharAbastecimento(ref mensagemErro, codigoFechamento, liberacaoAbastecimentoAutomatizado))
                        throw new Exception(@$"FecharAbastecimento: {mensagemErro}.");
                }
                else
                {
                    throw new Exception(@"FinalizarAbastecimento: Situação do abastecimento diferente de Em abastecimento");
                }

                repLiberacaoAbastecimentoAutomatizadoIntegracao.Atualizar(liberacaoAbastecimentoAutomatizadoIntegracao);
                repLiberacaoAbastecimentoAutomatizado.Atualizar(liberacaoAbastecimentoAutomatizado);

                return true;

            }
            catch (ServicoException excecao)
            {
                Log.TratarErro(excecao);

                return false;
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);

                return false;
            }

        }
        #endregion

        #region Métodos Privados

        private void RegistrarRequisicao(object json, string reserveID)
        {
            Repositorio.Embarcador.Integracao.IntegracaoConecttecRequisicao repIntegracaoConecttecRequisicao = new Repositorio.Embarcador.Integracao.IntegracaoConecttecRequisicao(_unitOfWork);
            Dominio.Entidades.Embarcador.Integracao.IntegracaoConecttecRequisicao integracaoConecttecRequisicao = new IntegracaoConecttecRequisicao();
            integracaoConecttecRequisicao.DataRequisicao = DateTime.Now;
            integracaoConecttecRequisicao.ReserveID = reserveID;
            integracaoConecttecRequisicao.JSON = Newtonsoft.Json.JsonConvert.SerializeObject(json);
            repIntegracaoConecttecRequisicao.Inserir(integracaoConecttecRequisicao);
        }

        private bool CriarRegistroAbastecimento(Dominio.ObjetosDeValor.WebService.Abastecimento.FinalizarAbastecimentoConecttec abs, Dominio.Entidades.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizado liberacaoAbastecimentoAutomatizado, Dominio.Entidades.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizadoIntegracao liberacaoAbastecimentoAutomatizadoIntegracao)
        {

            Repositorio.Embarcador.Produtos.LocalArmazenamentoProduto repLocalArmazenamentoProduto = new Repositorio.Embarcador.Produtos.LocalArmazenamentoProduto(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
            Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(_unitOfWork);
           
            Dominio.Entidades.Abastecimento novoAbastecimento = new Dominio.Entidades.Abastecimento();
  
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();

            try
            {
                novoAbastecimento.Data = liberacaoAbastecimentoAutomatizado?.DataAbastecimento ?? DateTime.Now;
                novoAbastecimento.Veiculo = liberacaoAbastecimentoAutomatizado.Veiculo;
                novoAbastecimento.Motorista = liberacaoAbastecimentoAutomatizado.Motorista;
                novoAbastecimento.TipoAbastecimento = TipoAbastecimento.Combustivel;
                novoAbastecimento.TipoRecebimentoAbastecimento = TipoRecebimentoAbastecimento.Interno;
                novoAbastecimento.BombaAbastecimento = liberacaoAbastecimentoAutomatizado.BombaAbastecimento;
                novoAbastecimento.Posto = liberacaoAbastecimentoAutomatizado.BombaAbastecimento.LocalArmazenamentoProduto.Posto;
                novoAbastecimento.Kilometragem = liberacaoAbastecimentoAutomatizado.QuilometragemAtual;
                novoAbastecimento.Produto = liberacaoAbastecimentoAutomatizado.BombaAbastecimento.LocalArmazenamentoProduto.TipoOleo.Produto;
                novoAbastecimento.Litros = Convert.ToDecimal(liberacaoAbastecimentoAutomatizadoIntegracao.QuantidadeAbastecida);
                novoAbastecimento.ValorUnitario = abs.Price ?? 0;
                novoAbastecimento.Situacao = "A";
                novoAbastecimento.LocalArmazenamento = liberacaoAbastecimentoAutomatizado.BombaAbastecimento.LocalArmazenamentoProduto;
                novoAbastecimento.Observacao = $"Abastecimento interno gerado através da liberação automática da bomba: {liberacaoAbastecimentoAutomatizado.BombaAbastecimento.Descricao} - Código de Autorização: {liberacaoAbastecimentoAutomatizadoIntegracao.AuthID}";
                novoAbastecimento.LiberacaoAbastecimentoAutomatizado = liberacaoAbastecimentoAutomatizado;


                Servicos.Embarcador.Abastecimento.Abastecimento.ValidarAbastecimentoInconsistente(ref novoAbastecimento, _unitOfWork, liberacaoAbastecimentoAutomatizado.Veiculo, null, configuracaoEmbarcador);

                repAbastecimento.Inserir(novoAbastecimento);

                return true;

            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);
                return false;
            }

        }

        private bool GerarFechamentoAbastecimento(ref string mensagemErro, ref int CodigoFechamento, Dominio.Entidades.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizado liberacaoAbastecimentoAutomatizado)
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_unitOfWork.StringConexao);
            Servicos.DTO.ParametrosFechamentoAbastecimento parametrosFechamentoAbastecimento = new Servicos.DTO.ParametrosFechamentoAbastecimento();
            Servicos.Embarcador.Abastecimento.Interfaces.IFechamentoAbastecimento ServicoFechamentoAbastecimento = new Servicos.Embarcador.Abastecimento.FechamentoAbastecimento(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS, _auditado, unitOfWork);
            Dominio.Entidades.Embarcador.Frotas.FechamentoAbastecimento fechamentoAbastecimento = new Dominio.Entidades.Embarcador.Frotas.FechamentoAbastecimento();
       
            try
            {
                parametrosFechamentoAbastecimento.CodigoVeiculo = liberacaoAbastecimentoAutomatizado.Veiculo.Codigo;
                parametrosFechamentoAbastecimento.CodigoPosto =  liberacaoAbastecimentoAutomatizado.BombaAbastecimento.LocalArmazenamentoProduto.Posto.Codigo;
                parametrosFechamentoAbastecimento.DataInicio  = liberacaoAbastecimentoAutomatizado?.DataAbastecimento ?? DateTime.Now;
                parametrosFechamentoAbastecimento.DataFim = liberacaoAbastecimentoAutomatizado?.DataAbastecimento ?? DateTime.Now;
                parametrosFechamentoAbastecimento.CodigosEmpresa = new List<int>();

                if (!ServicoFechamentoAbastecimento.GerarFechamentoAbastecimento(parametrosFechamentoAbastecimento, fechamentoAbastecimento, ref mensagemErro))
                    return false;

                CodigoFechamento = fechamentoAbastecimento.Codigo;

                return true;

            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);
                mensagemErro = ex.Message;
                return false;
            }

        }

        private bool FecharAbastecimento(ref string mensagemErro, int codigoFechamento ,Dominio.Entidades.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizado liberacaoAbastecimentoAutomatizado)
        {


            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_unitOfWork.StringConexao);

            Repositorio.Embarcador.Frotas.FechamentoAbastecimento repFechamentoAbastecimento = new Repositorio.Embarcador.Frotas.FechamentoAbastecimento(unitOfWork);
            Servicos.DTO.ParametrosFechamentoAbastecimento parametrosFechamentoAbastecimento = new Servicos.DTO.ParametrosFechamentoAbastecimento();
            
            Servicos.Embarcador.Abastecimento.Interfaces.IFechamentoAbastecimento ServicoFechamentoAbastecimento = new Servicos.Embarcador.Abastecimento.FechamentoAbastecimento(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS, _auditado, unitOfWork);
      
            try
            {

                parametrosFechamentoAbastecimento.CodigoFechamento = codigoFechamento;

                if (!ServicoFechamentoAbastecimento.FecharAbastecimento(parametrosFechamentoAbastecimento, ref mensagemErro))
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);
                mensagemErro = ex.Message;
                return false;
            }

        }

        #endregion

    }
}