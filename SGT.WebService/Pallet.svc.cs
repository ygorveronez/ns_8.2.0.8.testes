using CoreWCF;

namespace SGT.WebService
{
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any, IncludeExceptionDetailInFaults = true)]
    public class Pallet(IServiceProvider serviceProvider) : WebServiceBase(serviceProvider), IPallet
    {
        #region Métodos Globais

        public Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal>> BuscarNotasFiscaisComConfirmacaoRecebimentoPallet(int? inicio, int? limite)
        {
            ValidarToken();

            inicio ??= 0;
            limite ??= 0;

            Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal>> retorno = new Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal>>();
            retorno.Status = true;
            retorno.Objeto = new Paginacao<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal>();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                if (limite <= 100)
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                    retorno.Mensagem = "Método em implementação";
                }
                else
                {
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Status = false;
                    retorno.Mensagem = "O limite não pode ser maior que 100";
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao consultar as notas fiscais com recebimento de pallet confirmados";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<bool> ConfirmarRecebimentoNotaFiscalComConfirmacaoRecebimentoPallet(int protocoloNFe)
        {

            ValidarToken();

            Retorno<bool> retorno = new Retorno<bool>();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Método em implementação";
            }
            catch (Exception ex)
            {
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                retorno.Mensagem = "Ocorreu uma falha ao tentar enviar a confirmação do recebimento das notas fiscais com recebimento de pallet confirmados.";
            }
            finally
            {
                unitOfWork.Dispose();
            }
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }


        public Retorno<bool> MovimentacaoPallet(Dominio.ObjetosDeValor.WebService.Pallet.MovimentacaoPallet movimentacaoPallet)
        {
            Servicos.Log.TratarErro($"MovimentacaoPallet: {(movimentacaoPallet != null ? Newtonsoft.Json.JsonConvert.SerializeObject(movimentacaoPallet) : string.Empty)}", "Request");
            ValidarToken();

            Retorno<bool> retorno = new Retorno<bool>();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Embarcador.Pallets.DevolucaoPallet repDevolucao = new Repositorio.Embarcador.Pallets.DevolucaoPallet(unitOfWork);

                if (movimentacaoPallet == null)
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "Dados recebidos inválidos.";
                    retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    return retorno;
                }

                if (movimentacaoPallet.Quantidade < 0)
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "Quantidade de pallets deve ser maior ou igual a zero.";
                    retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    return retorno;
                }

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorProtocolo(movimentacaoPallet.ProtocoloCarga);
                if (carga == null)
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "Carga não encontrada com protocolo " + movimentacaoPallet.ProtocoloCarga.ToString();
                    retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    return retorno;
                }

                if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada || carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada)
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "Situação da Carga (" + carga.DescricaoSituacaoCarga + ") não permite movimentação da pallet.";
                    retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    return retorno;
                }

                Dominio.Entidades.Embarcador.Filiais.Filial filial = repFilial.buscarPorCodigoEmbarcador(movimentacaoPallet.Filial.CodigoIntegracao);
                if (filial == null)
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "Filial não encontrada com codigo de integração " + movimentacaoPallet.Filial.CodigoIntegracao.ToString();
                    retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    return retorno;
                }

                Dominio.Entidades.Empresa transportador = repEmpresa.BuscarPorCNPJ(movimentacaoPallet.Transportador.CNPJ);
                if (transportador == null)
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "Transportador não encontrada com CNPJ " + movimentacaoPallet.Transportador.CNPJ;
                    retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    return retorno;
                }

                Dominio.Entidades.Embarcador.Pallets.DevolucaoPallet devolucaoPallet = repDevolucao.BuscarPrimeiroPorCarga(carga.Codigo);
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoMovimentacaoEstoquePallet tipoMovimentacao = movimentacaoPallet.TipoMovimentacaoPallet == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentacaoPallet.Entrada ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoMovimentacaoEstoquePallet.TransportadorEntrada : Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoMovimentacaoEstoquePallet.TransportadorSaida;

                if (devolucaoPallet == null && movimentacaoPallet.TipoMovimentacaoPallet == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentacaoPallet.Saida)
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "Não é possível fazer movimentação de saída sem ter tido uma entrada.";
                    retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    return retorno;
                }

                DateTime dataMovimentacao = DateTime.Now;

                if (devolucaoPallet == null)
                {
                    int.TryParse(Utilidades.String.OnlyNumbers(movimentacaoPallet.NumeroDocumento), out int numeroDocumento);

                    devolucaoPallet = new Dominio.Entidades.Embarcador.Pallets.DevolucaoPallet();
                    devolucaoPallet.CargaPedido = repCargaPedido.BuscarPrimeiraPorCarga(carga.Codigo);
                    devolucaoPallet.Transportador = transportador;
                    devolucaoPallet.Filial = filial;
                    devolucaoPallet.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDevolucaoPallet.AgEntrega;
                    devolucaoPallet.NumeroDevolucao = numeroDocumento > 0 ? numeroDocumento : repDevolucao.BuscarProximoCodigo();
                    devolucaoPallet.QuantidadePallets = movimentacaoPallet.Quantidade;
                    devolucaoPallet.Observacao = movimentacaoPallet.NumeroDocumento;

                    if (DateTime.TryParseExact(movimentacaoPallet.DataMovimentacao, "dd/MM/yyyy hh:mm:ss", null, System.Globalization.DateTimeStyles.None, out dataMovimentacao))
                        devolucaoPallet.DataDevolucao = dataMovimentacao;

                    repDevolucao.Inserir(devolucaoPallet, Auditado);
                }
                else
                {
                    if (tipoMovimentacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoMovimentacaoEstoquePallet.TransportadorEntrada)
                        devolucaoPallet.QuantidadePallets += movimentacaoPallet.Quantidade;
                    else
                        devolucaoPallet.QuantidadePallets -= movimentacaoPallet.Quantidade;

                    if (devolucaoPallet.QuantidadePallets < 0)
                    {
                        retorno.Status = false;
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Mensagem = "Movimentação vai gerar saldo negativo, não é possível continuar.";
                        retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                        return retorno;
                    }

                    if (devolucaoPallet.Observacao != movimentacaoPallet.NumeroDocumento)
                        devolucaoPallet.Observacao = string.Concat(devolucaoPallet.Observacao, " / ", movimentacaoPallet.NumeroDocumento);
                    repDevolucao.Atualizar(devolucaoPallet, Auditado);
                }

                Servicos.Embarcador.Pallets.EstoquePallet servicoEstoquePallet = new Servicos.Embarcador.Pallets.EstoquePallet(unitOfWork, Auditado);

                string observacaoMovimentacao = movimentacaoPallet.Observacao;
                if (!string.IsNullOrWhiteSpace(movimentacaoPallet.NumeroDocumento))
                    observacaoMovimentacao = string.IsNullOrWhiteSpace(observacaoMovimentacao) ? "Referente ao documento " + movimentacaoPallet.NumeroDocumento : string.Concat(observacaoMovimentacao, " - ", "Referente ao documento " + movimentacaoPallet.NumeroDocumento);

                var dadosMovimentacaoEstoque = new Dominio.ObjetosDeValor.Embarcador.Pallets.DadosMovimentacaoEstoquePallet()
                {
                    CodigoFilial = filial.Codigo,
                    CodigoTransportador = transportador.Codigo,
                    Observacao = observacaoMovimentacao,
                    Quantidade = movimentacaoPallet.Quantidade,
                    TipoLancamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLancamento.Manual,
                    TipoOperacaoMovimentacao = tipoMovimentacao,
                    DataMovimento = dataMovimentacao,
                    CodigoGrupoPessoas = devolucaoPallet.CargaPedido?.Carga?.GrupoPessoaPrincipal?.Codigo ?? 0
                };

                unitOfWork.Start();

                servicoEstoquePallet.InserirMovimentacao(dadosMovimentacaoEstoque);

                unitOfWork.CommitChanges();

                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.Sucesso;
                retorno.Mensagem = "Movimentação efetuada com sucesso.";

            }
            catch (Exception ex)
            {
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                retorno.Mensagem = "Ocorreu uma falha ao enviar a movimentação de pallet.";
            }
            finally
            {
                unitOfWork.Dispose();
            }
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }


        public Retorno<bool> RetornoContestacaoPallet(Dominio.ObjetosDeValor.WebService.Pallet.RetornoContestacao retornoContestacao)
        {
            Servicos.Log.TratarErro($"ConfirmarContestacao: {(retornoContestacao != null ? Newtonsoft.Json.JsonConvert.SerializeObject(retornoContestacao) : string.Empty)}", "Request");
            ValidarToken();

            Retorno<bool> retorno = new Retorno<bool>();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                retorno.Mensagem = "Método em implementação";
            }
            catch (Exception ex)
            {
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                retorno.Mensagem = "Ocorreu uma falha ao tentar enviar a contestacao.";
            }
            finally
            {
                unitOfWork.Dispose();
            }
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }


        #endregion

        #region Métodos Privados


        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override Dominio.ObjetosDeValor.Enumerador.OrigemAuditado ObterOrigemAuditado()
        {
            return Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.WebServicePallet;
        }

        #endregion
    }
}
