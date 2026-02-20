using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Servicos.Embarcador.Pedido
{
    public sealed class ConferenciaContainer
    {
        #region Atributos

        private readonly Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        #region Construtores

        public ConferenciaContainer(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, auditado: null) { }

        public ConferenciaContainer(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            _auditado = auditado;
            _unitOfWork = unitOfWork;
        }

        #endregion Contrutores

        #region Métodos Privados

        public Dominio.Entidades.Embarcador.Pedidos.ConferenciaContainer AdicionarConferenciaContainer(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Pedidos.ConferenciaContainer repositorioConferenciaContainer = new Repositorio.Embarcador.Pedidos.ConferenciaContainer(_unitOfWork);
            Dominio.Entidades.Embarcador.Pedidos.ConferenciaContainer conferenciaContainer =  new Dominio.Entidades.Embarcador.Pedidos.ConferenciaContainer()
            {
                Carga = carga,
                Situacao = SituacaoConferenciaContainer.AguardandoConferencia
            };

            repositorioConferenciaContainer.Inserir(conferenciaContainer);
            Auditoria.Auditoria.AuditarSemDadosUsuario(conferenciaContainer, "Adicionada a carga para a conferência de container", _unitOfWork);

            return conferenciaContainer;
        }

        private void AuditarAlteracao(Dominio.Entidades.Embarcador.Pedidos.ConferenciaContainer conferenciaContainer, string descricaoAlteracao)
        {
            if (_auditado != null)
                Auditoria.Auditoria.Auditar(_auditado, conferenciaContainer, descricaoAlteracao, _unitOfWork);
            else
                Auditoria.Auditoria.AuditarSemDadosUsuario(conferenciaContainer, descricaoAlteracao, _unitOfWork);
        }

        private bool IsUtilizarConferenciaContainer(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (carga.TipoOperacao == null)
                return false;

            return carga.TipoOperacao.ObrigatorioVincularContainerCarga && carga.TipoOperacao.ObrigatorioRealizarConferenciaContainerCarga;
        }

        public Dominio.Entidades.Embarcador.Pedidos.ConferenciaContainer ObterConferenciaContainer(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Pedidos.ConferenciaContainer repositorioConferenciaContainer = new Repositorio.Embarcador.Pedidos.ConferenciaContainer(_unitOfWork);

            return repositorioConferenciaContainer.BuscarPorCarga(carga.Codigo);
        }

        private void RemoverConferenciaContainer(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Pedidos.ConferenciaContainer repositorioConferenciaContainer = new Repositorio.Embarcador.Pedidos.ConferenciaContainer(_unitOfWork);
            
            repositorioConferenciaContainer.DeletarPorCarga(carga.Codigo);
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public void Adicionar(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (!IsUtilizarConferenciaContainer(carga))
            {
                RemoverConferenciaContainer(carga);
                return;
            }

            if (new Repositorio.Embarcador.Pedidos.ConferenciaContainer(_unitOfWork).ExistePorCarga(carga.Codigo))
                return;

            AdicionarConferenciaContainer(carga);
        }

        public void Aprovar(int codigoConferenciaContainer, string observacao)
        {
            Repositorio.Embarcador.Pedidos.ConferenciaContainer repositorioConferenciaContainer = new Repositorio.Embarcador.Pedidos.ConferenciaContainer(_unitOfWork);
            Dominio.Entidades.Embarcador.Pedidos.ConferenciaContainer conferenciaContainer = repositorioConferenciaContainer.BuscarPorCodigo(codigoConferenciaContainer, auditavel: false);

            if (conferenciaContainer == null)
                throw new ServicoException("Não foi possível encontrar o registro");

            if (conferenciaContainer.Situacao != SituacaoConferenciaContainer.AguardandoConferencia)
                throw new ServicoException("A situação atual da conferência não permite a aprovação");

            conferenciaContainer.DataConferencia = DateTime.Now;
            conferenciaContainer.Observacao = observacao;
            conferenciaContainer.Situacao = SituacaoConferenciaContainer.ConferenciaAprovada;
            conferenciaContainer.UsuarioConferiu = _auditado?.Usuario;

            repositorioConferenciaContainer.Atualizar(conferenciaContainer);
            AuditarAlteracao(conferenciaContainer, $"Aprovada a conferência do container {conferenciaContainer.Container.Numero}");
        }

        public void Atualizar(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Pedidos.Container container)
        {
            if (!IsUtilizarConferenciaContainer(carga))
            {
                RemoverConferenciaContainer(carga);
                return;
            }

            Dominio.Entidades.Embarcador.Pedidos.ConferenciaContainer conferenciaContainer = ObterConferenciaContainer(carga);

            if (conferenciaContainer == null)
                conferenciaContainer = AdicionarConferenciaContainer(carga);

            if (conferenciaContainer.Container?.Codigo == container.Codigo)
                return;

            conferenciaContainer.Container = container;
            conferenciaContainer.DataConferencia = null;
            conferenciaContainer.Observacao = string.Empty;
            conferenciaContainer.Situacao = SituacaoConferenciaContainer.AguardandoConferencia;
            conferenciaContainer.UsuarioConferiu = null;

            new Repositorio.Embarcador.Pedidos.ConferenciaContainer(_unitOfWork).Atualizar(conferenciaContainer);
            AuditarAlteracao(conferenciaContainer, $"Adicionado o container {conferenciaContainer.Container.Numero} para a conferência");
        }

        public bool PossuiConferenciaSemAprovacao(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (!IsUtilizarConferenciaContainer(carga))
            {
                RemoverConferenciaContainer(carga);
                return false;
            }

            Dominio.Entidades.Embarcador.Pedidos.ConferenciaContainer conferenciaContainer = ObterConferenciaContainer(carga);

            return (conferenciaContainer != null) && (conferenciaContainer.Situacao != SituacaoConferenciaContainer.ConferenciaAprovada);
        }

        public void TrocarCarga(Dominio.Entidades.Embarcador.Cargas.Carga cargaAtual, Dominio.Entidades.Embarcador.Cargas.Carga cargaNova)
        {
            Dominio.Entidades.Embarcador.Pedidos.ConferenciaContainer conferenciaContainer = ObterConferenciaContainer(cargaAtual);
            if(conferenciaContainer != null)
            {
                conferenciaContainer.Carga = cargaNova;
                new Repositorio.Embarcador.Pedidos.ConferenciaContainer(_unitOfWork).Atualizar(conferenciaContainer);
            }
        }

        #endregion Métodos Públicos
    }
}
