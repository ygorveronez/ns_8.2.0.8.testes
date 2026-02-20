using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoCodigoExternoIntegracao
    {
        Pessoa = 0,
        PessoaEndereco = 1,
        Veiculo = 2,
        VeiculoMarca = 3,
        VeiculoModelo = 4,
        Localidade = 5,
        ModeloVeicular = 6,
        ModeloVeicularAgrupamento = 7,
        ContratoFrete = 8,
        PagamentoMotoristaTMS = 9,
    }

    public static class TipoCodigoExternoIntegracaooHelper
    {
        public static string ObterDescricao(this TipoCodigoExternoIntegracao tipo)
        {
            switch (tipo)
            {
                case TipoCodigoExternoIntegracao.Pessoa: return "Pessoa";
                case TipoCodigoExternoIntegracao.PessoaEndereco: return "Pessoa Endereço";
                case TipoCodigoExternoIntegracao.Veiculo: return "Veículo";
                case TipoCodigoExternoIntegracao.VeiculoMarca: return "Marca";
                case TipoCodigoExternoIntegracao.VeiculoModelo: return "Modelo";
                case TipoCodigoExternoIntegracao.Localidade: return "Localidade";
                case TipoCodigoExternoIntegracao.ModeloVeicular: return "Modelo Veicular";
                case TipoCodigoExternoIntegracao.ModeloVeicularAgrupamento: return "Modelo Veicular agrupamento";
                default: return string.Empty;
            }
        }
    }
}
