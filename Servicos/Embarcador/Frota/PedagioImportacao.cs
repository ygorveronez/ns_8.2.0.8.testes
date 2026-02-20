using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Servicos.Embarcador.Frota
{
    public sealed class PedagioImportacao
    {
        #region Atributos Privados Somente Leitura

        private readonly Dictionary<string, dynamic> _dados;
        private readonly AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;
        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly Dominio.Entidades.Empresa _empresa;

        #endregion

        #region Construtores

        public PedagioImportacao(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Empresa empresa, Dictionary<string, dynamic> dados)
        {
            _dados = dados;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
            _unitOfWork = unitOfWork;
            _empresa = empresa;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Pedagio.Pedagio ObterPedagioImportar()
        {
            Dominio.Entidades.Embarcador.Pedagio.Pedagio pedagio = new Dominio.Entidades.Embarcador.Pedagio.Pedagio();
            pedagio.DataImportacao = DateTime.Now.Date;
            pedagio.DataAlteracao = DateTime.Now;
            pedagio.ImportadoDeSemParar = true;

            pedagio.Veiculo = ObterVeiculo(out string placaVeiculoNaoCadastrado);
            pedagio.Data = ObterDataHoraPassagem();
            pedagio.TipoPedagio = ObterTipoPedagio(out string prefixo);
            pedagio.Prefixo = prefixo;
            pedagio.Valor = ObterValor(pedagio.TipoPedagio);
            pedagio.Praca = ObterPraca();
            pedagio.Rodovia = ObterRodovia();
            pedagio.Observacao = ObterObservacao();

            // Caso o veículo não esteja cadastrado ou seja de terceiros, o pedágio fica com Inconsistência
            if (pedagio.Veiculo != null)
            {
                if (pedagio.Veiculo.Tipo.ToUpper() == "P")
                    pedagio.SituacaoPedagio = SituacaoPedagio.Lancado;
                else
                {
                    pedagio.SituacaoPedagio = SituacaoPedagio.Inconsistente;
                    pedagio.TipoMovimento = ObterMovimentoFinanceiro();

                    if (pedagio.TipoMovimento != null)
                        pedagio.SituacaoPedagio = SituacaoPedagio.Lancado;

                }
            }
            else
            {
                pedagio.PlacaVeiculoNaoCadastrado = placaVeiculoNaoCadastrado;
                pedagio.SituacaoPedagio = SituacaoPedagio.Inconsistente;
            }

            if (_tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                pedagio.Empresa = _empresa;

            ValidarRegras(pedagio);

            return pedagio;
        }

        #endregion

        #region Métodos Privados

        private void ValidarRegras(Dominio.Entidades.Embarcador.Pedagio.Pedagio pedagio)
        {
            Repositorio.Embarcador.Pedagio.Pedagio repPedagio = new Repositorio.Embarcador.Pedagio.Pedagio(_unitOfWork);

            Dominio.Entidades.Embarcador.Pedagio.Pedagio pedagioDuplicado = repPedagio.BuscarPorDados(pedagio.TipoPedagio, pedagio.Veiculo?.Placa ?? string.Empty, pedagio.Data, pedagio.Rodovia, pedagio.Praca);
            if (pedagioDuplicado != null)
                throw new ImportacaoException("Pedágio duplicado, assim o mesmo não foi importado");
        }

        private Dominio.Entidades.Veiculo ObterVeiculo(out string placaVeiculoNaoCadastrado)
        {
            placaVeiculoNaoCadastrado = null;
            string placaBuscar = string.Empty;

            if (_dados.TryGetValue("Placa", out var placa))
                placaBuscar = (string)placa;

            if (string.IsNullOrWhiteSpace(placaBuscar))
                throw new ImportacaoException("Placa não informada");

            placaBuscar = placaBuscar.Trim().ToUpper();

            Repositorio.Veiculo repositorio = new Repositorio.Veiculo(_unitOfWork);
            Dominio.Entidades.Veiculo veiculo = repositorio.BuscarPorPlaca(placaBuscar);

            if (veiculo == null)
                placaVeiculoNaoCadastrado = placaBuscar;

            return veiculo;
        }

        private decimal ObterValor(TipoPedagio tipoPedagio)
        {
            decimal valorRetornar = 0;
            if (_dados.TryGetValue("Valor", out var valor))
                valorRetornar = ((string)valor).Replace("R$", "").Replace(" ", "").Trim().ToDecimal();

            if (valorRetornar == 0)
                throw new ImportacaoException("Valor não informado");

            if (tipoPedagio == TipoPedagio.Credito)
                valorRetornar = valorRetornar * -1;

            return valorRetornar;
        }

        private DateTime ObterDataHoraPassagem()
        {
            DateTime? data = null;
            TimeSpan? hora = null;
            if (_dados.TryGetValue("Data", out var dataConverter))
                data = ((string)dataConverter).ToNullableDateTime();

            if (_dados.TryGetValue("Hora", out var horaConverter))
                hora = ((string)horaConverter).ToNullableTime();

            if (data == null)
                throw new ImportacaoException("Data de passagem não informada");

            DateTime dataRetornar = data.Value;
            if (hora != null)
            {
                dataRetornar = dataRetornar.AddHours(hora.Value.Hours);
                dataRetornar = dataRetornar.AddMinutes(hora.Value.Minutes);
                dataRetornar = dataRetornar.AddSeconds(hora.Value.Seconds);
            }

            return dataRetornar;
        }

        private TipoPedagio ObterTipoPedagio(out string prefixo)
        {
            prefixo = null;
            TipoPedagio tipoPedagioRetornar = TipoPedagio.Todos;

            if (_dados.TryGetValue("Tipo", out var tipoConverter))
                prefixo = (string)tipoConverter;

            if (string.IsNullOrWhiteSpace(prefixo))
                throw new ImportacaoException("Prefixo do Tipo não informado");
            else if (prefixo.Equals("CR"))
                tipoPedagioRetornar = TipoPedagio.Credito;
            else if (prefixo.Equals("DB"))
                tipoPedagioRetornar = TipoPedagio.Debito;

            if (tipoPedagioRetornar == TipoPedagio.Todos)
                throw new ImportacaoException("Na coluna Tipo é esperado: CR - Crédito e DB - Débito");

            return tipoPedagioRetornar;
        }

        private string ObterPraca()
        {
            string pracaRetornar = string.Empty;

            if (_dados.TryGetValue("Praca", out var praca))
                pracaRetornar = (string)praca;

            return pracaRetornar;
        }

        private string ObterRodovia()
        {
            string rodoviaRetornar = string.Empty;

            if (_dados.TryGetValue("Rodovia", out var rodovia))
                rodoviaRetornar = (string)rodovia;

            return rodoviaRetornar;
        }

        private string ObterObservacao()
        {
            string observacaoRetornar = string.Empty;

            if (_dados.TryGetValue("Observacao", out var observacao))
                observacaoRetornar = (string)observacao;

            return observacaoRetornar;
        }

        private Dominio.Entidades.Embarcador.Financeiro.TipoMovimento ObterMovimentoFinanceiro()
        {
            int codigoMovimento = 0;
            Dominio.Entidades.Embarcador.Financeiro.TipoMovimento movimentoFinanceiroRetornar = new Dominio.Entidades.Embarcador.Financeiro.TipoMovimento();

            if (_dados.TryGetValue("MovimentoFinanceiro", out var movimentoFinanceiro))
            {
                int.TryParse(movimentoFinanceiro.ToString(), out codigoMovimento);
                Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(_unitOfWork);
                movimentoFinanceiroRetornar = repTipoMovimento.BuscarPorCodigo(codigoMovimento);
            }

            return movimentoFinanceiroRetornar;
        }

        #endregion
    }
}
