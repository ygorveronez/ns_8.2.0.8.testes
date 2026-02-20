using System;

namespace Servicos.Embarcador.Pessoa
{
    public class ColaboradorSituacaoLancamento : ServicoBase
    {
        #region Variaveis Privadas

        readonly Repositorio.UnitOfWork _unitOfWork;
        readonly Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        readonly protected string _adminStringConexao;

        #endregion

        #region Construtores

        public ColaboradorSituacaoLancamento(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, string adminStringConexao) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _auditado = auditado;
            _adminStringConexao = adminStringConexao;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.ObjetosDeValor.WebService.Retorno<bool> SalvarSituacaoColaborador(Dominio.ObjetosDeValor.Embarcador.Carga.SituacaoColaboradorIntegracaoWS situacaoColaboradorIntegracao)
        {
            _unitOfWork.Start();

            Repositorio.Usuario repUsuario = new Repositorio.Usuario(_unitOfWork);
            Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorLancamento repColaboradorLancamento = new Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorLancamento(_unitOfWork);
            Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorSituacao repColaboradorSituacao = new Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorSituacao(_unitOfWork);

            if (string.IsNullOrWhiteSpace(situacaoColaboradorIntegracao.Colaborador.CPF))
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos("CPF não foi informado.");

            if (!Utilidades.Validate.ValidarCPF(situacaoColaboradorIntegracao.Colaborador.CPF))
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos("CPF informado é inválido.");

            if (string.IsNullOrWhiteSpace(situacaoColaboradorIntegracao.Descricao))
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos("Descrição não foi informada.");

            if (situacaoColaboradorIntegracao.DataInicial == DateTime.MinValue)
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos("Data inicial inválida ou não informada.");

            if (situacaoColaboradorIntegracao.DataFinal == DateTime.MinValue || situacaoColaboradorIntegracao.DataFinal <= situacaoColaboradorIntegracao.DataInicial)
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos("Data final inválida ou não informada.");

            Dominio.Entidades.Usuario motorista = repUsuario.BuscarMotoristaPorCPF(situacaoColaboradorIntegracao.Colaborador.CPF);

            if (motorista == null)
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos("Motorista não foi encontrado.");

            Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorSituacao colaboradorSituacao = repColaboradorSituacao.BuscarPorCodigoIntegracao(situacaoColaboradorIntegracao.SituacaoColaborador.CodigoIntegracao);

            if (colaboradorSituacao == null)
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoDadosInvalidos("Colaborador Situação não encontrado.");

            Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorLancamento colaboradorLancamento = PreencherColaboradorLancamento(situacaoColaboradorIntegracao, motorista, colaboradorSituacao);
            repColaboradorLancamento.Inserir(colaboradorLancamento, _auditado);

            Servicos.Embarcador.Transportadores.Motorista.AtualizarStatusColaborador(_unitOfWork, _auditado, false, colaboradorLancamento.Colaborador.Codigo);
            Servicos.Embarcador.Transportadores.Motorista.AtualizarIntegracaoSituacaoColaborador(_unitOfWork, colaboradorLancamento.Codigo);

            if (colaboradorLancamento == null)
            {
                _unitOfWork.Rollback();
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoExcecao("Ocorreu um erro ao inserir registro.");
            }
            else
            {
                _unitOfWork.CommitChanges();
                return Dominio.ObjetosDeValor.WebService.Retorno<bool>.CriarRetornoSucesso(true);
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorLancamento PreencherColaboradorLancamento(Dominio.ObjetosDeValor.Embarcador.Carga.SituacaoColaboradorIntegracaoWS situacaoColaboradorIntegracao, Dominio.Entidades.Usuario motorista, Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorSituacao colaboradorSituacao)
        {
            Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorLancamento repColaboradorLancamento = new Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorLancamento(_unitOfWork);
            Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorLancamento colaboradorLancamento = new Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorLancamento();

            colaboradorLancamento.Descricao = situacaoColaboradorIntegracao.Descricao;

            if (colaboradorLancamento.Codigo == 0)
            {
                colaboradorLancamento.Data = DateTime.Now;
                colaboradorLancamento.Numero = repColaboradorLancamento.ProximoNumeroColaboradorLancamento();
                colaboradorLancamento.SituacaoLancamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoColaborador.Agendado;
            }

            colaboradorLancamento.DataInicial = situacaoColaboradorIntegracao.DataInicial;
            colaboradorLancamento.DataFinal = situacaoColaboradorIntegracao.DataFinal;
            colaboradorLancamento.ColaboradorSituacao = colaboradorSituacao;
            colaboradorLancamento.Colaborador = motorista;

            return colaboradorLancamento;
        }

        #endregion
    }
}
