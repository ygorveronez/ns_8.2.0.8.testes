using Repositorio;
using System;
using System.Linq;

namespace Servicos.Embarcador.Integracao.Agraria
{
    public class IntegracaoAgraria
    {
        private readonly UnitOfWork _unitOfWork;

        public IntegracaoAgraria(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public bool AutenticarChaveNFeMotorista()
        {
            return false;
        }

        public bool AutenticarNumeroPagerMotorista()
        {
            return false;
        }

        public bool IsNotaFiscal(string valorEntrada)
        {
            try
            {
                bool isChaveValida = Utilidades.Validate.ValidarChaveNFe(valorEntrada);

                return isChaveValida;
            }
            catch
            {
                return false;
            }
        }

        public bool IsOrdemEmbarque(string valor)
        {
            if (!valor.Contains("OE_"))
                return false;

            return true;
        }

        public Dominio.ObjetosDeValor.Embarcador.AcessoMotorista.AcessoMotoristaDadosCarga ObterDadosCarga(string ordemEmbarque)
        {
            Dominio.Entidades.Embarcador.Cargas.Carga carga = ObterCargaOrdemEmbarque(ordemEmbarque);
            
            if (carga == null)
                throw new Exception("Carga não encontrada.");

            if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada || carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada || carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada)
                throw new Exception("A situação da carga não permite o acesso.");

            return new Dominio.ObjetosDeValor.Embarcador.AcessoMotorista.AcessoMotoristaDadosCarga
            {
                CodigoCarga = carga.Codigo,
                NumeroCarga = carga.CodigoCargaEmbarcador,
                DescricaoMotorista = carga.DadosSumarizados?.Motoristas,
                Veiculo = carga.Veiculo?.Placa_Formatada,
                Destino = carga.Pedidos?.LastOrDefault()?.Destino?.Descricao,
                Peso = carga.DadosSumarizados?.PesoLiquidoTotal.ToString("n2"),
                HorarioCarregamento = carga.DataCarregamentoCarga?.ToString("dd/MM/yyyy HH:mm")
            };
        }

        private Dominio.Entidades.Embarcador.Cargas.Carga ObterCargaOrdemEmbarque(string ordemEmbarque)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

            int codigoCarga = ordemEmbarque.Replace("OE_", "").ToInt();
            
            return repositorioCarga.BuscarPorCodigo(codigoCarga);
        }
    }
}
