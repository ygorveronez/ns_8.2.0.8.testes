using System;
using System.Linq;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.CIOT.RepomFrete;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Pedidos;

namespace Servicos.Embarcador.CIOT.RepomFrete
{
    public partial class IntegracaoRepomFrete
    {
        #region Métodos Globais

        public SituacaoRetornoCIOT IntegrarCIOT(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);

            bool sucesso = false;
            string mensagemErro = string.Empty;
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT retIntContratoFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Pendencia;

            try
            {
                this.configuracaoIntegracaoRepomFrete = this.ObterConfiguracaoRepomFrete(ciot.ConfiguracaoCIOT, unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = repCargaCIOT.BuscarPrimeiroPorCIOT(ciot.Codigo);

                ciot.Operadora = Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.RepomFrete;

                if (ciot.Motorista == null)
                    ciot.Motorista = cargaCIOT.Carga.Motoristas.FirstOrDefault();

                if (ciot.Contratante == null)
                    ciot.Contratante = cargaCIOT.Carga.Empresa;

                if (ciot.Veiculo == null)
                {
                    ciot.Veiculo = cargaCIOT.Carga.Veiculo;
                    ciot.VeiculosVinculados = cargaCIOT.Carga.VeiculosVinculados.ToList();
                }

                Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro = Servicos.Embarcador.CIOT.CIOT.ObterModalidadeTransportador(ciot.Transportador, unitOfWork);
                Dominio.Entidades.Veiculo carreta = ciot.VeiculosVinculados.FirstOrDefault();
                Dominio.Entidades.Cliente proprietarioCarreta = null;
                Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiroCarreta = null;

                if (carreta != null && carreta.Tipo == "T" && carreta.Proprietario != null && carreta.Proprietario.CPF_CNPJ != ciot.Transportador.CPF_CNPJ)
                {
                    proprietarioCarreta = carreta.Proprietario;
                    modalidadeTerceiroCarreta = Servicos.Embarcador.CIOT.CIOT.ObterModalidadeTransportador(carreta.Proprietario, unitOfWork);
                }

                enumAcaoContratoFrete acaoContratoFrete = this.ObterAcaoContratoFrete(ciot);

                // Efetua o login na administradora e gera o token
                if (this.ObterToken(out mensagemErro))
                {
                    if (acaoContratoFrete == enumAcaoContratoFrete.IncluirContratoFrete)
                    {
                        string numeroCartao = modalidadeTerceiro.TipoFavorecidoCIOT == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFavorecidoCIOT.Motorista ? cargaCIOT.CIOT.Motorista.NumeroCartao : modalidadeTerceiro?.NumeroCartao;

                        Int64? pefCardNumber = null;
                        Int64 nCardNumber;
                        if (Int64.TryParse(numeroCartao, out nCardNumber))
                            pefCardNumber = nCardNumber;

                        #region Buscar Branch Code (Código da Filial Repom)
                        string branchCode = cargaCIOT.Carga.Pedidos?.FirstOrDefault()?.Pedido?.CentroDeCustoViagem?.CodigoFilialRepom;

                        if (string.IsNullOrEmpty(branchCode))
                            branchCode = cargaCIOT.Carga.Empresa.Configuracao.CodigoFilialRepom;
                        #endregion
                        string jsonEnvio = string.Empty;
                        string jsonRetorno = string.Empty;

                        if (IntegrarProprietario(ciot.Transportador, modalidadeTerceiro, unitOfWork, out mensagemErro, out jsonEnvio, out jsonRetorno) &&
                            IntegrarProprietario(proprietarioCarreta, modalidadeTerceiroCarreta, unitOfWork, out mensagemErro, out jsonEnvio, out jsonRetorno) &&
                            IntegrarMotorista(ciot.Transportador, ciot.Motorista, unitOfWork, out mensagemErro, out jsonEnvio, out jsonRetorno) &&
                            IntegrarVeiculo(ciot.Veiculo, false, unitOfWork, out mensagemErro, out jsonEnvio, out jsonRetorno) &&
                            IntegrarVeiculosVinculados(ciot.VeiculosVinculados.ToList(), true, unitOfWork, out mensagemErro, out jsonEnvio, out jsonRetorno) &&
                            ConsultarRoteiro(cargaCIOT.Carga, cargaCIOT.Carga.Rota, unitOfWork, out mensagemErro, out string codigoRoteiro, out string codigoPercurso, branchCode, out jsonEnvio, out jsonRetorno) &&
                            validarProprietarioContratoFrete(ciot.Transportador, modalidadeTerceiro, unitOfWork, out mensagemErro) &&
                            validarCartaoProprietarioContratoFrete(ciot.Transportador, modalidadeTerceiro, pefCardNumber, unitOfWork, out mensagemErro) &&
                            validarVeiculoContratoFrete(ciot.Veiculo, false, unitOfWork, out mensagemErro) &&
                            validarVeiculoContratoFrete(carreta, true, unitOfWork, out mensagemErro))
                        {
                            retIntContratoFrete = IntegrarContratoFrete(acaoContratoFrete, cargaCIOT, modalidadeTerceiro, codigoRoteiro, codigoPercurso, pefCardNumber, branchCode, tipoServicoMultisoftware, unitOfWork, out mensagemErro);

                            if (retIntContratoFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.AgIntegracao)
                                sucesso = true;
                        }
                    }
                    else
                    {
                        retIntContratoFrete = IntegrarContratoFrete(acaoContratoFrete, cargaCIOT, modalidadeTerceiro, null, null, null, null, tipoServicoMultisoftware, unitOfWork, out mensagemErro);

                        if (retIntContratoFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.AgIntegracao || retIntContratoFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Aberto)
                            sucesso = true;
                    }
                }
            }
            catch (ServicoException excecao)
            {
                Log.TratarErro(excecao);
                mensagemErro = $"Falha ao realizar a integração da Repom Frete: {excecao.Message}";
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
                mensagemErro = "Ocorreu uma falha ao realizar a integração da Repom Frete.";
            }

            if (!sucesso)
            {
                ciot.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Pendencia;
                ciot.Mensagem = mensagemErro;
            }

            if (ciot.Codigo > 0)
                repCIOT.Atualizar(ciot);
            else
                repCIOT.Inserir(ciot);

            if (sucesso && retIntContratoFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Aberto)
                return SituacaoRetornoCIOT.Autorizado;
            else if (sucesso)
                return SituacaoRetornoCIOT.EmProcessamento;
            else
                return SituacaoRetornoCIOT.ProblemaIntegracao;
        }

        public void AjustarReenviarCIOT(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, out string mensagemErro)
        {
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);

            mensagemErro = null;

            if (ciot.CIOTPorPeriodo && ciot.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Aberto)
            {
                enumAcaoContratoFrete acaoContratoFrete = this.ObterAcaoContratoFrete(cargaCIOT);
                if (acaoContratoFrete == enumAcaoContratoFrete.ConsultarStatusIncluirContratoFrete)
                {
                    bool sucesso = this.ConsultarViagem(ref cargaCIOT, unitOfWork, out mensagemErro);
                    if (sucesso && string.IsNullOrEmpty(cargaCIOT.CIOT.ProtocoloAutorizacao))
                    {
                        cargaCIOT.ProtocoloAbertura = null;
                        repCargaCIOT.Atualizar(cargaCIOT);
                    }
                }
                else if (acaoContratoFrete == enumAcaoContratoFrete.ConsultarStatusCIOTANTT)
                {
                    /*
                    if (CancelarCIOT(cargaCIOT, unitOfWork, out mensagemErro, false))
                    {
                        cargaCIOT.ProtocoloAbertura = null;
                        cargaCIOT.ProtocoloAutorizacao = null;
                        repCargaCIOT.Atualizar(cargaCIOT);
                    }
                    */
                }
            }
            else
            {
                enumAcaoContratoFrete acaoContratoFrete = this.ObterAcaoContratoFrete(ciot);
                if (acaoContratoFrete == enumAcaoContratoFrete.ConsultarStatusIncluirContratoFrete)
                {
                    bool sucesso = this.ConsultarViagem(ref ciot, unitOfWork, out mensagemErro);
                    if (sucesso && string.IsNullOrEmpty(ciot.ProtocoloAutorizacao))
                    {
                        ciot.ProtocoloAbertura = null;
                        repCIOT.Atualizar(ciot);
                    }
                }
                else if (acaoContratoFrete == enumAcaoContratoFrete.ConsultarStatusCIOTANTT)
                {
                    if (CancelarCIOT(ciot, unitOfWork, out mensagemErro, false))
                    {
                        ciot.ProtocoloAbertura = null;
                        ciot.ProtocoloAutorizacao = null;
                        repCIOT.Atualizar(ciot);
                    }
                }
            }
        }

        #endregion

        #region Métodos Privados

        #endregion
    }
}
