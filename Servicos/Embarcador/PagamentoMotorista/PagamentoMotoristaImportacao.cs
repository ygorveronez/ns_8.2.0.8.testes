using Dominio.Excecoes.Embarcador;
using System;
using System.Collections.Generic;

namespace Servicos.Embarcador.PagamentoMotorista
{
    public sealed class PagamentoMotoristaImportacao
    {
        #region Atributos Privados Somente Leitura

        private readonly Dictionary<string, dynamic> _dados;
        private readonly AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;
        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly Dominio.Entidades.Empresa _empresa;
        private readonly Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracao;

        #endregion

        #region Construtores

        public PagamentoMotoristaImportacao(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Empresa empresa, Dictionary<string, dynamic> dados, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            _dados = dados;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
            _unitOfWork = unitOfWork;
            _empresa = empresa;
            _configuracao = configuracao;
        }

        #endregion

        #region Métodos Privados

        private DateTime ObterDataPagamento()
        {
            DateTime? data = null;
            if (_dados.TryGetValue("DataPagamento", out var dataPagamento))
                data = ((string)dataPagamento).ToNullableDateTime();

            return data ?? DateTime.Now;
        }

        private Dominio.Entidades.Usuario ObterMotorista()
        {
            string motoristaBuscar = string.Empty;

            if (_dados.TryGetValue("CpfMotorista", out var cpfMotorista))
                motoristaBuscar = Utilidades.String.OnlyNumbers((string)cpfMotorista).PadLeft(11, '0');

            if (string.IsNullOrWhiteSpace(motoristaBuscar))
                throw new ImportacaoException("Motorista não informado.");

            Repositorio.Usuario repUsuario = new Repositorio.Usuario(_unitOfWork);
            Dominio.Entidades.Usuario motorista = repUsuario.BuscarPorCPF(motoristaBuscar);

            if (motorista == null)
                throw new ImportacaoException("Motorista não cadastrado no sistema! Favor fazer esse cadastro antes de prosseguir com a importação.");

            return motorista;
        }

        private decimal ObterValor()
        {
            decimal valor = 0;
            if (_dados.TryGetValue("Valor", out var valorImportado))
                valor = ((string)valorImportado).ToDecimal();

            if (valor == 0)
                throw new ImportacaoException("Valor não informado.");

            return valor;
        }

        private Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo ObterTipoPagamento()
        {
            var codigoIntegracaoTipoPagamento = string.Empty;

            if (_dados.TryGetValue("TipoPagamento", out var tipoPagamento))
                codigoIntegracaoTipoPagamento = ((string)tipoPagamento) != null ? ((string)tipoPagamento).Trim() : string.Empty; ;

            if (string.IsNullOrWhiteSpace(codigoIntegracaoTipoPagamento))
                throw new ImportacaoException("Tipo de Pagamento não informado.");

            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo repositorioTipoPagamento = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo(_unitOfWork);
            Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo tipoDoPagamento = repositorioTipoPagamento.BuscarPorCodigoIntegracao(codigoIntegracaoTipoPagamento);

            if (tipoDoPagamento == null)
                throw new ImportacaoException("Tipo do Pagamento não encontrado.");

            return tipoDoPagamento;
        }

        private string ObterObservacao()
        {
            var observacaoRetornar = string.Empty;

            if (_dados.TryGetValue("Observacao", out var observacao))
                observacaoRetornar = ((string)observacao) != null ? ((string)observacao).Trim() : string.Empty;

            return observacaoRetornar;
        }

        private Dominio.Entidades.Embarcador.Cargas.Carga ObterCarga()
        {
            var codigoCargaEmbarcador = string.Empty;

            if (_dados.TryGetValue("Carga", out var numeroCarga))
                codigoCargaEmbarcador = ((string)numeroCarga) != null ? ((string)numeroCarga).Trim() : string.Empty;

            if (!string.IsNullOrWhiteSpace(codigoCargaEmbarcador))
            {
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarCargaPorCodigoEmbarcador(codigoCargaEmbarcador);

                return carga;
            }

            return null;
        }

        private Dominio.Entidades.Embarcador.Chamados.Chamado ObterChamado()
        {
            int numeroChamado = 0;

            if (_dados.TryGetValue("Chamado", out var chamadoNumero))
                numeroChamado = ((string)chamadoNumero).ToInt();

            if (numeroChamado > 0)
            {
                Repositorio.Embarcador.Chamados.Chamado repositorioChamado = new Repositorio.Embarcador.Chamados.Chamado(_unitOfWork);
                Dominio.Entidades.Embarcador.Chamados.Chamado chamado = repositorioChamado.BuscarPorNumero(numeroChamado);

                return chamado;
            }

            return null;
        }

        private Dominio.Entidades.Embarcador.Financeiro.PlanoConta ObterPlanoSaida()
        {
            string codigoIntegracaoPlanoSaida = string.Empty;

            if (_dados.TryGetValue("PlanoSaida", out var planoSaida))
                codigoIntegracaoPlanoSaida = (string)planoSaida;

            if (string.IsNullOrWhiteSpace(codigoIntegracaoPlanoSaida))
                throw new ImportacaoException("Plano de Saída não informado.");

            Dominio.Entidades.Embarcador.Financeiro.PlanoConta planoDeSaida = ObterPlanoConta(codigoIntegracaoPlanoSaida);

            if (planoDeSaida == null)
                throw new ImportacaoException("Plano de Sáida não encontrado.");

            return planoDeSaida;
        }

        private Dominio.Entidades.Embarcador.Financeiro.PlanoConta ObterPlanoEntrada()
        {
            string codigoIntegracaoPlanoEntrada = string.Empty;

            if (_dados.TryGetValue("PlanoEntrada", out var planoEntrada))
                codigoIntegracaoPlanoEntrada = (string)planoEntrada;

            if (string.IsNullOrWhiteSpace(codigoIntegracaoPlanoEntrada))
                throw new ImportacaoException("Plano de Entrada não informado.");

            Dominio.Entidades.Embarcador.Financeiro.PlanoConta planoDeEntrada = ObterPlanoConta(codigoIntegracaoPlanoEntrada);

            if (planoDeEntrada == null)
                throw new ImportacaoException("Plano de Entrada não encontrado.");

            return planoDeEntrada;
        }

        private Dominio.Entidades.Embarcador.Financeiro.PlanoConta ObterPlanoConta(string codigoIntegracao)
        {
            Repositorio.Embarcador.Financeiro.PlanoConta repositorioPlanoConta = new Repositorio.Embarcador.Financeiro.PlanoConta(_unitOfWork);
            return repositorioPlanoConta.BuscarPorCodigoIntegracao(codigoIntegracao);
        }

        private DateTime? ObterDataVencimentoTituloPagar()
        {
            DateTime? data = null;
            if (_dados.TryGetValue("DataVencimentoTituloPagar", out var dataPagamento))
                data = ((string)dataPagamento).ToNullableDateTime();

            return data;
        }

        private void ValidarDados(Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamentoMotorista)
        {
            if ((pagamentoMotorista.PagamentoMotoristaTipo.GerarTituloPagar || pagamentoMotorista.PagamentoMotoristaTipo.GerarTituloAPagarAoMotorista) && !pagamentoMotorista.DataVencimentoTituloPagar.HasValue)
                throw new ImportacaoException("Deve ser informada a data do Vencimento Tit. Pagar.");
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS ObterPagamentoMotoristaImportar(Dominio.Entidades.Usuario usuario)
        {
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS repositorioPagamentoMotorista = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS(_unitOfWork);
            Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamentoMotorista = new Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS();

            pagamentoMotorista.Chamado = ObterChamado();
            pagamentoMotorista.Carga = ObterCarga();
            pagamentoMotorista.Motorista = ObterMotorista();
            pagamentoMotorista.DataPagamento = ObterDataPagamento();
            pagamentoMotorista.PagamentoMotoristaTipo = ObterTipoPagamento();
            pagamentoMotorista.Valor = ObterValor();
            pagamentoMotorista.PlanoDeContaDebito = ObterPlanoSaida();
            pagamentoMotorista.PlanoDeContaCredito = ObterPlanoEntrada();
            pagamentoMotorista.Observacao = ObterObservacao();
            pagamentoMotorista.DataVencimentoTituloPagar = ObterDataVencimentoTituloPagar();

            pagamentoMotorista.Usuario = usuario;
            pagamentoMotorista.SituacaoPagamentoMotorista = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.AgInformacoes;
            pagamentoMotorista.Data = DateTime.Now.Date;
            pagamentoMotorista.EtapaPagamentoMotorista = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaPagamentoMotorista.Iniciada;
            pagamentoMotorista.PagamentoLiberado = true;

            ValidarDados(pagamentoMotorista);

            return pagamentoMotorista;
        }

        #endregion
    }
}
