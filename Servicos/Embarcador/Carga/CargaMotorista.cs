using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Carga
{
    public sealed class CargaMotorista
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        #region Construtores

        public CargaMotorista(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Públicos

        public void AdicionarMotorista(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Usuario motorista)
        {
            if (motorista == null)
                return;

            AdicionarMotoristas(carga, new List<Dominio.Entidades.Usuario>() { motorista });
        }

        public void AdicionarMotoristas(Dominio.Entidades.Embarcador.Cargas.Carga cargaReferencia, Dominio.Entidades.Embarcador.Cargas.Carga cargaNova)
        {
            Repositorio.Embarcador.Cargas.CargaMotorista repositorioCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(_unitOfWork);
            List<Dominio.Entidades.Usuario> motoristas = repositorioCargaMotorista.BuscarMotoristasPorCarga(cargaReferencia.Codigo);

            AdicionarMotoristas(cargaNova, motoristas);
        }

        public void AdicionarMotoristas(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Usuario> motoristas)
        {
            Repositorio.Embarcador.Cargas.CargaMotorista repositorioCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(_unitOfWork);

            foreach (Dominio.Entidades.Usuario motorista in motoristas)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaMotorista cargaMotorista = new Dominio.Entidades.Embarcador.Cargas.CargaMotorista()
                {
                    Carga = carga,
                    Motorista = motorista
                };

                repositorioCargaMotorista.Inserir(cargaMotorista);
            }
        }

        public bool AtualizarMotorista(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Usuario motorista)
        {
            List<Dominio.Entidades.Usuario> motoristas = new List<Dominio.Entidades.Usuario>();

            if (motorista != null)
                motoristas.Add(motorista);

            return AtualizarMotoristas(carga, motoristas);
        }

        public bool AtualizarMotoristas(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Usuario> motoristas, bool reenviarIntegracao = true, List<int> codigosMotoristasObjeto = null, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = null)
        {
            if (motoristas == null)
                motoristas = new List<Dominio.Entidades.Usuario>();

            Repositorio.Embarcador.Cargas.CargaMotorista repositorioCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(_unitOfWork);
            Servicos.Embarcador.Carga.MensagemAlertaCarga servicoMensagemAlerta = new Servicos.Embarcador.Carga.MensagemAlertaCarga(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaMotorista> cargaMotoristas = repositorioCargaMotorista.BuscarPorCarga(carga.Codigo);
            List<int> codigosMotoristas = motoristas.Select(o => o.Codigo).ToList();

            if (codigosMotoristasObjeto?.Count > 0)
                codigosMotoristas = codigosMotoristasObjeto;

            List<int> codigosMotoristasCarga = cargaMotoristas.Select(o => o.Motorista.Codigo).ToList();
            List<Dominio.Entidades.Embarcador.Cargas.CargaMotorista> cargaMotoristasRemover = cargaMotoristas.Where(o => !codigosMotoristas.Contains(o.Motorista.Codigo)).ToList();
            List<Dominio.Entidades.Usuario> motoristasAdicionar = motoristas.Where(o => !codigosMotoristasCarga.Contains(o.Codigo)).ToList();
            List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade> alteracoes = new List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade>();

            bool atualizouMotorista = false;

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaMotorista cargaMotoristaRemover in cargaMotoristasRemover)
            {
                alteracoes.Add(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                {
                    Propriedade = "Motoristas",
                    De = $"{cargaMotoristaRemover.Motorista.Codigo} - {cargaMotoristaRemover.Motorista.Descricao}",
                    Para = ""
                });

                repositorioCargaMotorista.Deletar(cargaMotoristaRemover);
                atualizouMotorista = true;
            }

            foreach (Dominio.Entidades.Usuario motoristaAdicionar in motoristasAdicionar)
            {
                alteracoes.Add(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                {
                    Propriedade = "Motoristas",
                    De = "",
                    Para = $"{motoristaAdicionar.Codigo} - {motoristaAdicionar.Descricao}"
                });

                Dominio.Entidades.Embarcador.Cargas.CargaMotorista cargaMotorista = new Dominio.Entidades.Embarcador.Cargas.CargaMotorista()
                {
                    Carga = carga,
                    Motorista = motoristaAdicionar
                };

                repositorioCargaMotorista.Inserir(cargaMotorista);
                atualizouMotorista = true;
            }

            if (carga.DataLimiteConfirmacaoMotorista.HasValue && servicoMensagemAlerta.IsMensagemSemConfirmacao(carga, TipoMensagemAlerta.CargaSemConfirmacaoMotorista))
            {
                TimeSpan tempoLimite = carga.Carregamento?.TempoLimiteConfirmacaoMotorista.TotalSeconds > 0 ? carga.Carregamento.TempoLimiteConfirmacaoMotorista : carga.TipoOperacao?.ConfiguracaoMobile?.TempoLimiteConfirmacaoMotorista ?? new TimeSpan();

                if (tempoLimite.TotalSeconds > 0)
                    carga.DataLimiteConfirmacaoMotorista = DateTime.Now.Add(tempoLimite);
            }

            if (atualizouMotorista && reenviarIntegracao)
                Servicos.Embarcador.Integracao.IntegracaoCarga.ReenviarIntegracaoDadosTransporte(carga, TipoIntegracao.Trizy, _unitOfWork, auditado);

            alteracoes = Servicos.Auditoria.Auditoria.CombinarAlteracoes(alteracoes);
            carga.SetExternalChanges(alteracoes);
            return atualizouMotorista;
        }

        public List<int> GerarNotificacaoMotoristaMobile(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Cargas.CargaMotorista repositorioCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaMotorista> cargaMotoristas = repositorioCargaMotorista.BuscarPorCarga(carga.Codigo);
            List<int> motoristasNotificar = new List<int>();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaMotorista cargaMotorista in cargaMotoristas)
            {
                if (cargaMotorista.Motorista.CodigoMobile <= 0)
                    continue;

                cargaMotorista.NotificacaoAtualizacaoCargaPendente = true;

                repositorioCargaMotorista.Atualizar(cargaMotorista);
                motoristasNotificar.Add(cargaMotorista.Motorista.CodigoMobile);
            }

            return motoristasNotificar;
        }

        #endregion Métodos Públicos
    }
}
