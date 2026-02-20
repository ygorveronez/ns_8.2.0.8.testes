using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Servicos.Embarcador.Transportadores
{
    public sealed class MotoristaGR
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        #region Contrutores

        public MotoristaGR(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Públicos

        public Dominio.ObjetosDeValor.Embarcador.GR.RetornoGR Validar(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Usuario motorista, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Dominio.ObjetosDeValor.Embarcador.GR.RetornoGR retornoTelerisco = ValidarTelerisco(carga, motorista, tipoServicoMultisoftware);
            Dominio.ObjetosDeValor.Embarcador.GR.RetornoGR retornoBrasilRisk = ValidarBrasilRisk(carga, motorista);
            Dominio.ObjetosDeValor.Embarcador.GR.RetornoGR retornoAdagio = ValidarAdagio(carga, motorista);
            Dominio.ObjetosDeValor.Embarcador.GR.RetornoGR retornoBuonny = ValidarBuonny(carga, motorista, tipoServicoMultisoftware);

            return retornoTelerisco ?? retornoBrasilRisk ?? retornoAdagio ?? retornoBuonny;
        }

        public Dominio.ObjetosDeValor.Embarcador.GR.RetornoGR ValidarAdagio(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Usuario motorista)
        {
            if ((motorista == null) || !(carga.TipoOperacao?.ValidarMotoristaVeiculoAdagioAoConfirmarTransportador ?? false))
                return null;

            Dominio.ObjetosDeValor.Embarcador.GR.RetornoGR retorno = new Dominio.ObjetosDeValor.Embarcador.GR.RetornoGR();
            bool falhaIntegracaoGrMotorista = false;

            retorno.Mensagem = Servicos.Embarcador.Transportadores.Motorista.ConsultarMotoristaAdagio(motorista, ref falhaIntegracaoGrMotorista, _unitOfWork);
            retorno.Sucesso = !falhaIntegracaoGrMotorista;
            retorno.TipoIntegracao = TipoIntegracao.Adagio;

            if (!retorno.Sucesso)
                retorno.Mensagem = $"{retorno.TipoIntegracao.Value.ObterDescricao()} Motorista {motorista.CPF}: {retorno.Mensagem}";

            return retorno;
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.GR.RetornoGR ValidarBrasilRisk(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Usuario motorista)
        {
            if ((motorista == null) || !(carga.TipoOperacao?.ValidarMotoristaVeiculoBrasilRiskAoConfirmarTransportador ?? false))
                return null;

            Dominio.ObjetosDeValor.Embarcador.GR.RetornoGR retorno = new Dominio.ObjetosDeValor.Embarcador.GR.RetornoGR();
            bool falhaIntegracaoGrMotorista = false;

            retorno.Mensagem = Servicos.Embarcador.Transportadores.Motorista.ConsultarMotoristaBrasilRisk(motorista, ref falhaIntegracaoGrMotorista, carga.Codigo, _unitOfWork);
            retorno.Sucesso = !falhaIntegracaoGrMotorista;
            retorno.TipoIntegracao = (!retorno.Sucesso) ? TipoIntegracao.BrasilRisk : TipoIntegracao.NaoInformada;

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.GR.RetornoGR ValidarTelerisco(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Usuario motorista, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if ((motorista == null) || !(carga.TipoOperacao?.ValidarMotoristaTeleriscoAoConfirmarTransportador ?? false) || !(carga.Empresa?.ValidarMotoristaTeleriscoAoConfirmarTransportador ?? false))
                return null;

            Dominio.ObjetosDeValor.Embarcador.GR.RetornoGR retorno = new Dominio.ObjetosDeValor.Embarcador.GR.RetornoGR();
            bool falhaIntegracaoGrMotorista = false;
            string protocolo = string.Empty;

            retorno.Mensagem = Servicos.Embarcador.Transportadores.Motorista.ConsultarMotoristaTelerisco(motorista, carga.Filial, carga.DataCarregamentoCarga ?? DateTime.MinValue, ref falhaIntegracaoGrMotorista, ref protocolo, tipoServicoMultisoftware, _unitOfWork, carga?.Veiculo?.Placa);
            retorno.Protocolo = protocolo;
            retorno.Sucesso = !falhaIntegracaoGrMotorista;
            retorno.TipoIntegracao = (!retorno.Sucesso) ? TipoIntegracao.Telerisco : TipoIntegracao.NaoInformada;

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.GR.RetornoGR ValidarBuonny(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Usuario motorista, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if ((motorista == null) || !(carga.TipoOperacao?.ValidarMotoristaBuonnyAoConfirmarTransportador ?? false))
                return null;

            Dominio.ObjetosDeValor.Embarcador.GR.RetornoGR retorno = new Dominio.ObjetosDeValor.Embarcador.GR.RetornoGR();
            bool falhaIntegracaoGrMotorista = false;
            string protocolo = string.Empty;

            retorno.Mensagem = Servicos.Embarcador.Transportadores.Motorista.ConsultarMotoristaBuonny(carga, motorista, ref falhaIntegracaoGrMotorista, ref protocolo, tipoServicoMultisoftware, _unitOfWork);
            retorno.Protocolo = protocolo;
            retorno.Sucesso = !falhaIntegracaoGrMotorista;
            retorno.TipoIntegracao = (!retorno.Sucesso) ? TipoIntegracao.Buonny : TipoIntegracao.NaoInformada;

            if (falhaIntegracaoGrMotorista && !string.IsNullOrEmpty(retorno.Mensagem))
                new Carga.MensagemAlertaCarga(_unitOfWork).Adicionar(carga, TipoMensagemAlerta.GerenciadoraRisco, retorno.Mensagem);

            return retorno;

        }

        #endregion
    }
}
