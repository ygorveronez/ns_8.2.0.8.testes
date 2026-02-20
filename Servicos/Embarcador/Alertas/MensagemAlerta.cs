using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Alertas
{
    public abstract class MensagemAlerta<TMensagemAlerta, TEntidadeMensagemAlerta>
        where TMensagemAlerta : Dominio.Entidades.Embarcador.Alertas.MensagemAlerta<TEntidadeMensagemAlerta>, new()
        where TEntidadeMensagemAlerta : Dominio.Entidades.EntidadeBase, Dominio.Interfaces.Embarcador.Entidade.IEntidade
    {
        #region Atributos

        protected readonly Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        protected readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        #region Construtores

        public MensagemAlerta(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            _auditado = auditado;
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Privados

        private void AuditarConfirmacao(TMensagemAlerta mensagemAlerta)
        {
            if (_auditado == null)
                return;

            string mensagem = $"O usuário {_auditado.Usuario.Descricao} confirmou as mensagens de {mensagemAlerta.Tipo.ObterTituloMensagem().ToLower()}";

            Auditoria.Auditoria.Auditar(_auditado, mensagemAlerta.Entidade, mensagem, _unitOfWork);
        }

        private void AuditarRemocaoConfirmacao(TMensagemAlerta mensagemAlerta)
        {
            if (_auditado == null)
                return;

            string mensagem = $"O usuário {_auditado.Usuario.Descricao} removeu a confirmação das mensagens de {mensagemAlerta.Tipo.ObterTituloMensagem().ToLower()}";

            Auditoria.Auditoria.Auditar(_auditado, mensagemAlerta.Entidade, mensagem, _unitOfWork);
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public TMensagemAlerta Adicionar(TEntidadeMensagemAlerta entidade, TipoMensagemAlerta tipo, string mensagem)
        {
            return Adicionar(entidade, tipo, bloquear: false, new List<string>() { mensagem }, confirmada: false);
        }

        public TMensagemAlerta Adicionar(TEntidadeMensagemAlerta entidade, TipoMensagemAlerta tipo, List<string> mensagens)
        {
            return Adicionar(entidade, tipo, bloquear: false, mensagens, confirmada: false);
        }

        public TMensagemAlerta Adicionar(TEntidadeMensagemAlerta entidade, TipoMensagemAlerta tipo, List<string> mensagens, bool confirmada)
        {
            return Adicionar(entidade, tipo, bloquear: false, mensagens, confirmada);
        }

        public TMensagemAlerta Adicionar(TEntidadeMensagemAlerta entidade, TipoMensagemAlerta tipo, bool bloquear, string mensagem)
        {
            return Adicionar(entidade, tipo, bloquear, new List<string>() { mensagem }, confirmada: false);
        }

        public TMensagemAlerta Adicionar(TEntidadeMensagemAlerta entidade, TipoMensagemAlerta tipo, bool bloquear, List<string> mensagens, bool confirmada)
        {
            if ((mensagens == null) || (mensagens.Count == 0))
                return null;

            Repositorio.Embarcador.Alertas.MensagemAlerta<TMensagemAlerta, TEntidadeMensagemAlerta> repositorioMensagemAlerta = new Repositorio.Embarcador.Alertas.MensagemAlerta<TMensagemAlerta, TEntidadeMensagemAlerta>(_unitOfWork);
            TMensagemAlerta mensagemAlerta = new TMensagemAlerta()
            {
                Bloquear = bloquear,
                Confirmada = confirmada,
                Entidade = entidade,
                Tipo = tipo
            };

            mensagemAlerta.Mensagens = mensagens;
            repositorioMensagemAlerta.Inserir(mensagemAlerta);

            return mensagemAlerta;
        }

        public TMensagemAlerta AdicionarNaoDuplicado(TEntidadeMensagemAlerta entidade, TipoMensagemAlerta tipo, string mensagem, bool substituirMensagem = false)
        {
            Repositorio.Embarcador.Alertas.MensagemAlerta<TMensagemAlerta, TEntidadeMensagemAlerta> repositorioMensagemAlerta = new Repositorio.Embarcador.Alertas.MensagemAlerta<TMensagemAlerta, TEntidadeMensagemAlerta>(_unitOfWork);
            TMensagemAlerta mensagemAlerta = repositorioMensagemAlerta.BuscarPrimeiraNaoConfirmadaPorEntidadeETipo(entidade.Codigo, tipo);

            if (mensagemAlerta == null)
                return Adicionar(entidade, tipo, bloquear: false, new List<string>() { mensagem }, confirmada: false);

            if (substituirMensagem && !mensagemAlerta.Mensagens.Contains(mensagem))
            {
                mensagemAlerta.Mensagens = new List<string>() { mensagem };
                repositorioMensagemAlerta.Atualizar(mensagemAlerta);
                return mensagemAlerta;
            }

            return null;
        }

        public TMensagemAlerta AdicionarNaoDuplicado(TEntidadeMensagemAlerta entidade, TipoMensagemAlerta tipo, List<string> mensagens, bool substituirMensagens = false)
        {
            Repositorio.Embarcador.Alertas.MensagemAlerta<TMensagemAlerta, TEntidadeMensagemAlerta> repositorioMensagemAlerta = new Repositorio.Embarcador.Alertas.MensagemAlerta<TMensagemAlerta, TEntidadeMensagemAlerta>(_unitOfWork);
            TMensagemAlerta mensagemAlerta = repositorioMensagemAlerta.BuscarPrimeiraNaoConfirmadaPorEntidadeETipo(entidade.Codigo, tipo);

            if (mensagemAlerta == null)
                return Adicionar(entidade, tipo, bloquear: false, mensagens, confirmada: false);

            if (substituirMensagens)
            {
                IEnumerable<string> comp1 = mensagens.Except(mensagemAlerta.Mensagens);
                IEnumerable<string> comp2 = mensagemAlerta.Mensagens.Except(mensagens);

                if (comp1.Any() || comp2.Any())
                {
                    mensagemAlerta.Mensagens = mensagens;
                    repositorioMensagemAlerta.Atualizar(mensagemAlerta);
                    return mensagemAlerta;
                }
            }

            return null;
        }

        public void AdicionarMensagem(TEntidadeMensagemAlerta entidade, TipoMensagemAlerta tipo, string mensagem)
        {
            Repositorio.Embarcador.Alertas.MensagemAlerta<TMensagemAlerta, TEntidadeMensagemAlerta> repositorioMensagemAlerta = new Repositorio.Embarcador.Alertas.MensagemAlerta<TMensagemAlerta, TEntidadeMensagemAlerta>(_unitOfWork);
            TMensagemAlerta mensagemAlerta = repositorioMensagemAlerta.BuscarPrimeiraNaoConfirmadaPorEntidadeETipo(entidade.Codigo, tipo);

            if (mensagemAlerta == null)
            {
                mensagemAlerta = new TMensagemAlerta()
                {
                    Entidade = entidade,
                    Tipo = tipo
                };
            }

            mensagemAlerta.Mensagens.Add(mensagem);

            if (mensagemAlerta.Codigo > 0)
                repositorioMensagemAlerta.Atualizar(mensagemAlerta);
            else
                repositorioMensagemAlerta.Inserir(mensagemAlerta);
        }

        public void Confirmar(TMensagemAlerta mensagemAlerta)
        {
            Repositorio.Embarcador.Alertas.MensagemAlerta<TMensagemAlerta, TEntidadeMensagemAlerta> repositorioMensagemAlerta = new Repositorio.Embarcador.Alertas.MensagemAlerta<TMensagemAlerta, TEntidadeMensagemAlerta>(_unitOfWork);

            mensagemAlerta.Confirmada = true;
            mensagemAlerta.DataConfirmacao = DateTime.Now;
            mensagemAlerta.UsuarioConfirmacao = _auditado?.Usuario;

            repositorioMensagemAlerta.Atualizar(mensagemAlerta);
            AuditarConfirmacao(mensagemAlerta);
        }

        public void Confirmar(TEntidadeMensagemAlerta entidade, TipoMensagemAlerta tipo)
        {
            Repositorio.Embarcador.Alertas.MensagemAlerta<TMensagemAlerta, TEntidadeMensagemAlerta> repositorioMensagemAlerta = new Repositorio.Embarcador.Alertas.MensagemAlerta<TMensagemAlerta, TEntidadeMensagemAlerta>(_unitOfWork);
            List<TMensagemAlerta> mensagensAlerta = repositorioMensagemAlerta.BuscarNaoConfirmadasPorEntidadeETipo(entidade.Codigo, tipo);

            if (mensagensAlerta.Count == 0)
                return;

            foreach (TMensagemAlerta mensagemAlerta in mensagensAlerta)
                Confirmar(mensagemAlerta);
        }

        public void ConfirmarPorCodigo(int codigoMensagemAlerta)
        {
            Repositorio.Embarcador.Alertas.MensagemAlerta<TMensagemAlerta, TEntidadeMensagemAlerta> repositorioMensagemAlerta = new Repositorio.Embarcador.Alertas.MensagemAlerta<TMensagemAlerta, TEntidadeMensagemAlerta>(_unitOfWork);
            TMensagemAlerta mensagemAlerta = repositorioMensagemAlerta.BuscarPorCodigo(codigoMensagemAlerta, auditavel: false);

            if (mensagemAlerta == null)
                throw new ServicoException("Não foi possível encontrar o registro.");

            if (mensagemAlerta.Confirmada)
                throw new ServicoException("A confirmação já foi realizada.");

            Confirmar(mensagemAlerta);
        }
        public async Task ConfirmarPorCodigoAsync(int codigoMensagemAlerta)
        {
            Repositorio.Embarcador.Alertas.MensagemAlerta<TMensagemAlerta, TEntidadeMensagemAlerta> repositorioMensagemAlerta = new Repositorio.Embarcador.Alertas.MensagemAlerta<TMensagemAlerta, TEntidadeMensagemAlerta>(_unitOfWork);
            TMensagemAlerta mensagemAlerta = await repositorioMensagemAlerta.BuscarPorCodigoAsync(codigoMensagemAlerta, auditavel: false);

            if (mensagemAlerta == null)
                throw new ServicoException("Não foi possível encontrar o registro.");

            if (mensagemAlerta.Confirmada)
                throw new ServicoException("A confirmação já foi realizada.");

            Confirmar(mensagemAlerta);
        }

        public bool IsBloquearEntidade(int codigoEntidade)
        {
            Repositorio.Embarcador.Alertas.MensagemAlerta<TMensagemAlerta, TEntidadeMensagemAlerta> repositorioMensagemAlerta = new Repositorio.Embarcador.Alertas.MensagemAlerta<TMensagemAlerta, TEntidadeMensagemAlerta>(_unitOfWork);

            return repositorioMensagemAlerta.ExisteNaoConfirmadaComBloqueio(codigoEntidade);
        }

        public bool IsMensagemSemConfirmacao(TEntidadeMensagemAlerta entidade, TipoMensagemAlerta tipo)
        {
            Repositorio.Embarcador.Alertas.MensagemAlerta<TMensagemAlerta, TEntidadeMensagemAlerta> repositorioMensagemAlerta = new Repositorio.Embarcador.Alertas.MensagemAlerta<TMensagemAlerta, TEntidadeMensagemAlerta>(_unitOfWork);

            return repositorioMensagemAlerta.ExisteNaoConfirmadaPorEntidadeETipo(entidade.Codigo, tipo);
        }

        public bool IsMensagemSemConfirmacao(TEntidadeMensagemAlerta entidade, List<TipoMensagemAlerta> tipos)
        {
            Repositorio.Embarcador.Alertas.MensagemAlerta<TMensagemAlerta, TEntidadeMensagemAlerta> repositorioMensagemAlerta = new Repositorio.Embarcador.Alertas.MensagemAlerta<TMensagemAlerta, TEntidadeMensagemAlerta>(_unitOfWork);

            return repositorioMensagemAlerta.ExisteNaoConfirmadaPorEntidadeETipos(entidade.Codigo, tipos);
        }

        public void Remover(TEntidadeMensagemAlerta entidade, TipoMensagemAlerta tipo)
        {
            Repositorio.Embarcador.Alertas.MensagemAlerta<TMensagemAlerta, TEntidadeMensagemAlerta> repositorioMensagemAlerta = new Repositorio.Embarcador.Alertas.MensagemAlerta<TMensagemAlerta, TEntidadeMensagemAlerta>(_unitOfWork);
            List<TMensagemAlerta> mensagensAlerta = repositorioMensagemAlerta.BuscarNaoConfirmadasPorEntidadeETipo(entidade.Codigo, tipo);

            if (mensagensAlerta.Count == 0)
                return;

            foreach (TMensagemAlerta mensagemAlerta in mensagensAlerta)
                repositorioMensagemAlerta.Deletar(mensagemAlerta);
        }

        public void RemoverConfirmacao(TEntidadeMensagemAlerta entidade, TipoMensagemAlerta tipo)
        {
            Repositorio.Embarcador.Alertas.MensagemAlerta<TMensagemAlerta, TEntidadeMensagemAlerta> repositorioMensagemAlerta = new Repositorio.Embarcador.Alertas.MensagemAlerta<TMensagemAlerta, TEntidadeMensagemAlerta>(_unitOfWork);
            List<TMensagemAlerta> mensagensAlerta = repositorioMensagemAlerta.BuscarConfirmadasPorEntidadeETipo(entidade.Codigo, tipo);

            if (mensagensAlerta.Count == 0)
                return;

            foreach (TMensagemAlerta mensagemAlerta in mensagensAlerta)
            {
                mensagemAlerta.Confirmada = false;
                mensagemAlerta.DataConfirmacao = null;
                mensagemAlerta.UsuarioConfirmacao = null;

                repositorioMensagemAlerta.Atualizar(mensagemAlerta);
                AuditarRemocaoConfirmacao(mensagemAlerta);
            }
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Alertas.MensagemAlerta> ObterMensagensPorEntidade(int codigoEntidade)
        {
            return ObterMensagensPorEntidades(new List<int>() { codigoEntidade });
        }

        public virtual List<Dominio.ObjetosDeValor.Embarcador.Alertas.MensagemAlerta> ObterMensagensPorEntidades(List<int> codigosEntidades)
        {
            Repositorio.Embarcador.Alertas.MensagemAlerta<TMensagemAlerta, TEntidadeMensagemAlerta> repositorioMensagemAlerta = new Repositorio.Embarcador.Alertas.MensagemAlerta<TMensagemAlerta, TEntidadeMensagemAlerta>(_unitOfWork);
            List<TMensagemAlerta> mensagensAlerta = repositorioMensagemAlerta.BuscarNaoConfirmadasPorEntidades(codigosEntidades);
            List<Dominio.ObjetosDeValor.Embarcador.Alertas.MensagemAlerta> mensagensAlertaRetornar = new List<Dominio.ObjetosDeValor.Embarcador.Alertas.MensagemAlerta>();

            foreach (TMensagemAlerta mensagemAlerta in mensagensAlerta)
                mensagensAlertaRetornar.Add(new Dominio.ObjetosDeValor.Embarcador.Alertas.MensagemAlerta()
                {
                    Bloquear = mensagemAlerta.Bloquear,
                    Codigo = mensagemAlerta.Codigo,
                    CodigoEntidade = mensagemAlerta.Entidade.Codigo,
                    Mensagens = mensagemAlerta.Mensagens.ToList(),
                    Tipo = mensagemAlerta.Tipo,
                    Titulo = mensagemAlerta.Titulo,
                    UtilizarConfirmacao = mensagemAlerta.UtilizarConfirmacao
                });

            return mensagensAlertaRetornar;
        }

        #endregion Métodos Públicos
    }
}
