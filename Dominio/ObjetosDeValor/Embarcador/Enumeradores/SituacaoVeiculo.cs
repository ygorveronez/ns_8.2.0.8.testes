namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoVeiculo
    {
        Vazio = 1,
        AvisoCarregamento = 2,
        EmViagem = 3,
        EmManutencao = 4,
        Disponivel = 5,
        EmFila = 6,
        Indisponivel = 7
    }


    public static class SituacaoVeiculoHelper
    {
        public static string ObterDescricao(this SituacaoVeiculo situacaoVeiculo)
        {
            switch (situacaoVeiculo)
            {
                case ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo.Vazio:
                    return "Vazio";
                case ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo.AvisoCarregamento:
                    return "Aviso Carregamento";
                case ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo.EmFila:
                    return "Em Fila";
                case ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo.EmManutencao:
                    return "Em Manutenção";
                case ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo.EmViagem:
                    return "Em Viagem";
                case ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo.Indisponivel:
                    return "Indisponível";
                default:
                    return "Disponível";
            }
        }
    }
    }
