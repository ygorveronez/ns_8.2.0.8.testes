using System;
using System.Linq;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.CIOT.TruckPad;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Servicos.Embarcador.CIOT.TruckPad
{
    public partial class IntegracaoTruckPad
    {
        #region Métodos Globais

        public SituacaoRetornoCIOT IntegrarCIOT(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);

            bool sucesso = false;
            string mensagemErro = string.Empty;

            try
            {
                this._configuracaoIntegracaoTruckPad = this.ObterConfiguracaoTruckPad(ciot.ConfiguracaoCIOT, unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = repCargaCIOT.BuscarPrimeiroPorCIOT(ciot.Codigo);

                ciot.Operadora = Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.TruckPad;

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

                // Efetua o login na administradora e gera o token
                if (this.ObterToken(out mensagemErro))
                {
                    string numeroCartao = modalidadeTerceiro.TipoFavorecidoCIOT == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFavorecidoCIOT.Motorista ? cargaCIOT.CIOT.Motorista.NumeroCartao : modalidadeTerceiro?.NumeroCartao;

                    Int64? pefCardNumber = null;
                    Int64 nCardNumber;
                    if (Int64.TryParse(numeroCartao, out nCardNumber))
                        pefCardNumber = nCardNumber;

                    string jsonEnvio = string.Empty;
                    string jsonRetorno = string.Empty;


                    if (IntegrarContratoFrete(cargaCIOT, modalidadeTerceiro, pefCardNumber, tipoServicoMultisoftware, unitOfWork, out mensagemErro))
                        sucesso = true;
                    
                }
            }
            catch (ServicoException excecao)
            {
                Log.TratarErro(excecao);
                mensagemErro = $"Falha ao realizar a integração da TruckPad: {excecao.Message}";
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
                mensagemErro = "Ocorreu uma falha ao realizar a integração da TruckPad.";
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

            if (sucesso)
                return SituacaoRetornoCIOT.Autorizado;
            else
                return SituacaoRetornoCIOT.ProblemaIntegracao;
        }

        #endregion

        #region Métodos Privados

        #endregion
    }
}
