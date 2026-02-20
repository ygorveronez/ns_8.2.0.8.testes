using Dominio.Excecoes.Embarcador;
using System;
using System.Collections.Generic;

namespace Servicos.Embarcador.Logistica
{
    public class ImportacaoPedidoProgramacaoColeta
    {
        #region Atributos Privados Somente Leitura

        public Dictionary<string, dynamic> _dados;
        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public ImportacaoPedidoProgramacaoColeta(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Privados

        private int ObterAgrupamento()
        {
            int agrupamentoRetornar = 0;

            if (_dados.TryGetValue("Agrupamento", out var agrupamento))
                agrupamentoRetornar = ((string)agrupamento).ToInt();

            if (agrupamentoRetornar == 0)
                throw new ImportacaoException("Agrupamento não informado");

            return agrupamentoRetornar;
        }

        private int ObterSequencia()
        {
            if (_dados.TryGetValue("Sequencia", out var sequencia))
                return ((string)sequencia).ToInt();

            return 0;
        }

        private Dominio.Entidades.Cliente ObterRemetente()
        {
            string codigoIntegracaoBuscar = string.Empty;

            if (_dados.TryGetValue("CodigoIntegracaoRemetente", out var codigoIntegracao))
                codigoIntegracaoBuscar = (string)codigoIntegracao;

            if (string.IsNullOrWhiteSpace(codigoIntegracaoBuscar))
                throw new ImportacaoException("Código de integração do remetente não informado");

            Repositorio.Cliente repositorio = new Repositorio.Cliente(_unitOfWork);
            Dominio.Entidades.Cliente cliente = repositorio.BuscarPorCodigoIntegracao(codigoIntegracaoBuscar);

            if (cliente == null)
                throw new ImportacaoException("Remetente não encontrado");

            return cliente;
        }

        private decimal ObterDistancia()
        {
            if (_dados.TryGetValue("Distancia", out var distancia))
                return ((string)distancia).ToDecimal();

            return 0m;
        }

        private decimal ObterQuantidadePlanejada()
        {
            if (_dados.TryGetValue("QuantidadePlanejada", out var quantidadePlanejada))
                return ((string)quantidadePlanejada).ToDecimal();

            return 0m;
        }

        private Dominio.Entidades.Veiculo ObterVeiculo()
        {
            string placaBuscar = string.Empty;

            if (_dados.TryGetValue("Placa", out var placa))
                placaBuscar = (string)placa;

            if (string.IsNullOrWhiteSpace(placaBuscar))
                throw new ImportacaoException("Placa não informada");

            placaBuscar = placaBuscar.Replace("-", "").Trim().ToUpper();
            if (placaBuscar.Length != 7)
                throw new ImportacaoException("Placa deve ter 7 dígitos");

            Repositorio.Veiculo repositorio = new Repositorio.Veiculo(_unitOfWork);
            Dominio.Entidades.Veiculo veiculo = repositorio.BuscarPorPlacaComFetch(placaBuscar);

            if (veiculo == null)
                throw new ImportacaoException("Veículo não encontrado");

            return veiculo;
        }

        private DateTime ObterDataCarregamento()
        {
            DateTime data = DateTime.MinValue;

            _dados.TryGetValue("DataCarregamento", out var dataCarregamento);
            _dados.TryGetValue("HoraCarregamento", out var horaCarregamento);
            string dataString = (string)dataCarregamento;
            string horaString = (string)horaCarregamento;

            double dataFormatoExcel = (double)Utilidades.Decimal.Converter(dataString);
            if (dataFormatoExcel > 0)
                data = Utilidades.DateTime.ConverterDataExcelToDateTime(dataFormatoExcel, dataString);
            else if (!string.IsNullOrWhiteSpace(dataString))
                data = dataString.ToDateTime();

            if (!string.IsNullOrWhiteSpace(horaString))
            {
                DateTime hora;
                double horaFormatoExcel = (double)Utilidades.Decimal.Converter(horaString);
                if (horaFormatoExcel > 0)
                {
                    hora = DateTime.FromOADate(horaFormatoExcel);
                    data = new DateTime(data.Year, data.Month, data.Day, hora.Hour, hora.Minute, hora.Second);
                }
                else if (!string.IsNullOrWhiteSpace(horaString) && DateTime.TryParse(horaString, out hora))
                    data = new DateTime(data.Year, data.Month, data.Day, hora.Hour, hora.Minute, hora.Second);
            }

            if (data == DateTime.MinValue)
                throw new ImportacaoException("Data Carregamento não informada");

            return data;
        }

        private Dominio.Entidades.Empresa ObterTransportador()
        {
            string codigoIntegracaoBuscar = string.Empty;

            if (_dados.TryGetValue("CodigoIntegracaoTransportador", out var codigoIntegracao))
                codigoIntegracaoBuscar = (string)codigoIntegracao;

            if (string.IsNullOrWhiteSpace(codigoIntegracaoBuscar))
                throw new ImportacaoException("Código de integração do transportador não informado");

            Repositorio.Empresa repositorio = new Repositorio.Empresa(_unitOfWork);
            Dominio.Entidades.Empresa empresa = repositorio.BuscarPorCodigoIntegracaoComFetch(codigoIntegracaoBuscar);

            if (empresa == null)
                throw new ImportacaoException("Transportador não encontrado");

            if (empresa.Status == "I")
                throw new ImportacaoException("Transportador está inativo");

            return empresa;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.ObjetosDeValor.Embarcador.Logistica.ImportacaoPedidoProgramacaoColeta ObterPedidoImportar(Dictionary<string, dynamic> dados)
        {
            _dados = dados;

            Dominio.ObjetosDeValor.Embarcador.Logistica.ImportacaoPedidoProgramacaoColeta pedido = new Dominio.ObjetosDeValor.Embarcador.Logistica.ImportacaoPedidoProgramacaoColeta();

            try
            {
                pedido.Agrupamento = ObterAgrupamento();
                pedido.Sequencia = ObterSequencia();
                pedido.Remetente = ObterRemetente();
                pedido.Distancia = ObterDistancia();
                pedido.QuantidadePlanejada = ObterQuantidadePlanejada();
                pedido.Veiculo = ObterVeiculo();
                pedido.DataCarregamento = ObterDataCarregamento();
                pedido.Transportador = ObterTransportador();

                pedido.Sucesso = true;
            }
            catch (BaseException ex)
            {
                pedido.MensagemFalha = ex.Message;
            }
            catch (Exception)
            {
                pedido.MensagemFalha = "Ocorreu uma falha ao importar o registro";
            }

            return pedido;
        }

        #endregion
    }
}
