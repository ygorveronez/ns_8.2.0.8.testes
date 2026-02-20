using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Linq;

namespace Servicos.Embarcador.CIOT
{
    public partial class Ambipar
    {
        #region Métodos Globais

        public SituacaoRetornoCIOT IntegrarCIOT(Dominio.Entidades.Embarcador.Documentos.CIOT ciot)
        {
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(_unitOfWork);
            Repositorio.Embarcador.CIOT.CIOTAmbipar repConfiguracaoIntegracaoAmbipar = new Repositorio.Embarcador.CIOT.CIOTAmbipar(_unitOfWork);

            Dominio.Entidades.Embarcador.CIOT.CIOTAmbipar configuracao = repConfiguracaoIntegracaoAmbipar.BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = repCargaCIOT.BuscarPrimeiroPorCIOT(ciot.Codigo);
            this.ObterConfiguracaoAmbipar(ciot.ConfiguracaoCIOT);

            ciot.Operadora = OperadoraCIOT.Ambipar;

            if (ciot.Contratante == null)
                ciot.Contratante = cargaCIOT.Carga.Empresa;

            if (ciot.Motorista == null)
            {
                Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(_unitOfWork);

                Dominio.Entidades.Usuario veiculoMotorista = null;

                if (cargaCIOT.Carga.Veiculo != null)
                    veiculoMotorista = repVeiculoMotorista.BuscarMotoristaPrincipal(cargaCIOT.Carga.Veiculo.Codigo);

                ciot.Motorista = cargaCIOT.Carga.Motoristas != null && cargaCIOT.Carga.Motoristas.Count > 0 ? cargaCIOT.Carga.Motoristas.FirstOrDefault() : veiculoMotorista ?? null;
            }

            Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro = Servicos.Embarcador.CIOT.CIOT.ObterModalidadeTransportador(ciot.Transportador, _unitOfWork);
            Dominio.Entidades.Veiculo carreta = ciot.VeiculosVinculados.FirstOrDefault();
            Dominio.Entidades.Cliente proprietarioCarreta = null;
            Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiroCarreta = null;

            if (carreta != null && carreta.Tipo == "T" && carreta.Proprietario != null && carreta.Proprietario.CPF_CNPJ != ciot.Transportador.CPF_CNPJ)
            {
                proprietarioCarreta = carreta.Proprietario;
                modalidadeTerceiroCarreta = Servicos.Embarcador.CIOT.CIOT.ObterModalidadeTransportador(carreta.Proprietario, _unitOfWork);
            }

            bool sucesso = false;
            string mensagemErro = string.Empty;
            int? idTransportadorCIOT = null;
            int? idTransportadorCarreta = null;
            int? roteiroID = null;
            int? idMotorista = null;
            int? idVeiculo = null;
            int? idCarreta = null;

            if (IntegrarTransportador(ciot.Transportador, modalidadeTerceiro, _unitOfWork, out mensagemErro, out idTransportadorCIOT) &&
                IntegrarTransportador(proprietarioCarreta, modalidadeTerceiroCarreta, _unitOfWork, out mensagemErro, out idTransportadorCarreta) &&
                IntegrarMotorista(ciot.Transportador, ciot.Motorista, ciot.Transportador, idTransportadorCIOT, _unitOfWork, out mensagemErro, out idMotorista) &&
                IntegrarVeiculo(ciot.Veiculo, false, ciot.Transportador, idTransportadorCIOT, _unitOfWork, out mensagemErro, out idVeiculo) &&
                IntegrarVeiculo(carreta, true, ciot.Transportador, idTransportadorCarreta, _unitOfWork, out mensagemErro, out idCarreta) &&
                IntegrarRota(cargaCIOT.Carga, cargaCIOT.Carga.Rota, _unitOfWork, out mensagemErro, out roteiroID) &&
                IntegrarContratoFrete(out mensagemErro, cargaCIOT, roteiroID))
            {
                sucesso = true;
            }

            if (!sucesso)
            {
                ciot.Situacao = SituacaoCIOT.Pendencia;
                ciot.Mensagem = mensagemErro;
            }

            if (ciot.Codigo > 0)
                repCIOT.Atualizar(ciot);
            else
                repCIOT.Inserir(ciot);

            return sucesso ? SituacaoRetornoCIOT.Autorizado : SituacaoRetornoCIOT.ProblemaIntegracao;
        }

        #endregion

        #region Métodos Privados

        #endregion
    }
}