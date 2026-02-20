using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Servicos.Embarcador.Frete
{
    public sealed class ContratoPrestacaoServicoSaldo
    {
        #region Atributos Privados Somente Leitura

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly Dominio.Entidades.Usuario _usuario;

        #endregion

        #region Construtores

        public ContratoPrestacaoServicoSaldo(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, usuario: null) { }

        public ContratoPrestacaoServicoSaldo(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Usuario usuario)
        {
            _unitOfWork = unitOfWork;
            _usuario = usuario;
        }

        #endregion

        #region Métodos Privados

        private Dominio.Entidades.Embarcador.Frete.ContratoPrestacaoServico ObterContratoPrestacaoServico(Dominio.ObjetosDeValor.Embarcador.Frete.ContratoPrestacaoServicoSaldoDados dados)
        {
            Repositorio.Embarcador.Frete.ContratoPrestacaoServico repositorioContratoPrestacaoServico = new Repositorio.Embarcador.Frete.ContratoPrestacaoServico(_unitOfWork);
            Dominio.Entidades.Embarcador.Frete.ContratoPrestacaoServico contratoPrestacaoServico = null;

            if (dados.TipoLancamento == TipoLancamento.Manual)
                contratoPrestacaoServico = repositorioContratoPrestacaoServico.BuscarPorCodigo(dados.CodigoContratoPrestacaoServico);
            else
                contratoPrestacaoServico = repositorioContratoPrestacaoServico.BuscarAtivoPorTransportadorEFilial(dados.CodigoTransportador, dados.CodigoFilial);

            return contratoPrestacaoServico;
        }

        #endregion

        #region Métodos Públicos

        public void Adicionar(Dominio.ObjetosDeValor.Embarcador.Frete.ContratoPrestacaoServicoSaldoDados dados)
        {
            Dominio.Entidades.Embarcador.Frete.ContratoPrestacaoServico contratoPrestacaoServico = ObterContratoPrestacaoServico(dados) ?? throw new ServicoException("Contrato de prestação de serviço não encontrado");
            Repositorio.Embarcador.Frete.ContratoPrestacaoServicoSaldo repositorioContratoPrestacaoServicoSaldo = new Repositorio.Embarcador.Frete.ContratoPrestacaoServicoSaldo(_unitOfWork);
            decimal valorUtilizadoAtual = repositorioContratoPrestacaoServicoSaldo.ObterValorUtilizadoPorContratoPrestacaoServico(contratoPrestacaoServico.Codigo);
            decimal valorUtilizado = (valorUtilizadoAtual + (dados.TipoMovimentacao == TipoMovimentacaoContratoPrestacaoServico.Saida ? dados.Valor : -dados.Valor));

            if (valorUtilizado > contratoPrestacaoServico.ValorTeto)
                throw new ServicoException("O valor informado ultrapassa o saldo disponível do contrato de prestação de serviço");

            Dominio.Entidades.Embarcador.Frete.ContratoPrestacaoServicoSaldo contratoPrestacaoServicoSaldo = new Dominio.Entidades.Embarcador.Frete.ContratoPrestacaoServicoSaldo()
            {
                ContratoPrestacaoServico = contratoPrestacaoServico,
                Data = DateTime.Now,
                Descricao = dados.Descricao,
                TipoLancamento = dados.TipoLancamento,
                TipoMovimentacao = dados.TipoMovimentacao,
                Usuario = _usuario,
                Valor = dados.Valor,
                ValorUtilizado = dados.Valor + valorUtilizadoAtual
            };

            repositorioContratoPrestacaoServicoSaldo.Inserir(contratoPrestacaoServicoSaldo);
        }

        public void AdicionarSemValidacao(Dominio.ObjetosDeValor.Embarcador.Frete.ContratoPrestacaoServicoSaldoDados dados)
        {
            Dominio.Entidades.Embarcador.Frete.ContratoPrestacaoServico contratoPrestacaoServico = ObterContratoPrestacaoServico(dados);

            if (contratoPrestacaoServico == null)
                return;

            Repositorio.Embarcador.Frete.ContratoPrestacaoServicoSaldo repositorioContratoPrestacaoServicoSaldo = new Repositorio.Embarcador.Frete.ContratoPrestacaoServicoSaldo(_unitOfWork);
            decimal valorUtilizadoAtual = repositorioContratoPrestacaoServicoSaldo.ObterValorUtilizadoPorContratoPrestacaoServico(contratoPrestacaoServico.Codigo);
            decimal valorUtilizado = (valorUtilizadoAtual + (dados.TipoMovimentacao == TipoMovimentacaoContratoPrestacaoServico.Saida ? dados.Valor : -dados.Valor));

            if (valorUtilizado > contratoPrestacaoServico.ValorTeto)
                return;

            Dominio.Entidades.Embarcador.Frete.ContratoPrestacaoServicoSaldo contratoPrestacaoServicoSaldo = new Dominio.Entidades.Embarcador.Frete.ContratoPrestacaoServicoSaldo()
            {
                ContratoPrestacaoServico = contratoPrestacaoServico,
                Data = DateTime.Now,
                Descricao = dados.Descricao,
                TipoLancamento = dados.TipoLancamento,
                TipoMovimentacao = dados.TipoMovimentacao,
                Usuario = _usuario,
                Valor = dados.Valor,
                ValorUtilizado = dados.Valor + valorUtilizadoAtual
            };

            repositorioContratoPrestacaoServicoSaldo.Inserir(contratoPrestacaoServicoSaldo);
        }

        public bool IsPossuiSaldoDisponivel(Dominio.ObjetosDeValor.Embarcador.Frete.ContratoPrestacaoServicoSaldoDados dados)
        {
            Dominio.Entidades.Embarcador.Frete.ContratoPrestacaoServico contratoPrestacaoServico = ObterContratoPrestacaoServico(dados);

            if (contratoPrestacaoServico == null)
                return false;

            Repositorio.Embarcador.Frete.ContratoPrestacaoServicoSaldo repositorioContratoPrestacaoServicoSaldo = new Repositorio.Embarcador.Frete.ContratoPrestacaoServicoSaldo(_unitOfWork);
            decimal valorUtilizadoAtual = repositorioContratoPrestacaoServicoSaldo.ObterValorUtilizadoPorContratoPrestacaoServico(contratoPrestacaoServico.Codigo);
            decimal valorUtilizado = (valorUtilizadoAtual + (dados.TipoMovimentacao == TipoMovimentacaoContratoPrestacaoServico.Saida ? dados.Valor : -dados.Valor));

            return (valorUtilizado <= contratoPrestacaoServico.ValorTeto);
        }

        public bool IsUtilizaContratoPrestacaoServico()
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);

            return repositorioConfiguracao.BuscarConfiguracaoPadrao()?.UtilizarContratoPrestacaoServico ?? false;
        }

        public void Remover(int codigoContratoPrestacaoServicoSaldo)
        {
            Repositorio.Embarcador.Frete.ContratoPrestacaoServicoSaldo repositorioContratoPrestacaoServicoSaldo = new Repositorio.Embarcador.Frete.ContratoPrestacaoServicoSaldo(_unitOfWork);
            Dominio.Entidades.Embarcador.Frete.ContratoPrestacaoServicoSaldo contratoPrestacaoServicoSaldo = repositorioContratoPrestacaoServicoSaldo.BuscarPorCodigo(codigoContratoPrestacaoServicoSaldo);

            if (contratoPrestacaoServicoSaldo == null)
                throw new ServicoException("Não foi possível encontrar o registro");

            if (contratoPrestacaoServicoSaldo.TipoLancamento == TipoLancamento.Automatico)
                throw new ServicoException("Não é possível remover lançamentos automáticos.");

            decimal valorAjustar = (contratoPrestacaoServicoSaldo.TipoMovimentacao == TipoMovimentacaoContratoPrestacaoServico.Saida) ? -contratoPrestacaoServicoSaldo.Valor : contratoPrestacaoServicoSaldo.Valor;

            try
            {
                _unitOfWork.Start();

                repositorioContratoPrestacaoServicoSaldo.Deletar(contratoPrestacaoServicoSaldo);
                repositorioContratoPrestacaoServicoSaldo.AjustarValorUtilizado(contratoPrestacaoServicoSaldo.ContratoPrestacaoServico.Codigo, contratoPrestacaoServicoSaldo.Data, valorAjustar);

                _unitOfWork.CommitChanges();
            }
            catch(Exception)
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        #endregion
    }
}
