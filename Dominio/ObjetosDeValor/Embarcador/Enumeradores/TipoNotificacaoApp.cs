namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoNotificacaoApp
    {
        MotoristaDentroDoRaioDoFornecedor = 0,
        MotoristaForaDoRaioDoFornecedor = 1,
        CarregamentoEmAvaliacaoPelaLogistica = 2,
        CarregamentoValidadoAguardandoCompraValePedagio = 3,
        ValePedagioCompradoComSucesso = 4,
        ValePedagioNaoComprado = 5,
        MotoristaPodeSeguirViagem = 6,
        TratativaDoAtendimento = 7,
        RejeicaoDadosNFeColeta = 8,
        Custom = 99,
    }

    public static class TipoNotificacaoAppHelper
    {
        public static string ObterDescricao(this TipoNotificacaoApp tipo)
        {
            switch (tipo)
            {
                case TipoNotificacaoApp.MotoristaDentroDoRaioDoFornecedor: return "Foto da voleta registrada dentro do raio da origem";
                case TipoNotificacaoApp.MotoristaForaDoRaioDoFornecedor: return "Foto da voleta registrada fora do raio da origem";
                case TipoNotificacaoApp.CarregamentoEmAvaliacaoPelaLogistica: return "Foto da coleta recebida";
                case TipoNotificacaoApp.CarregamentoValidadoAguardandoCompraValePedagio: return "Carregamento avaliado aguardando compra de Vale pedágio";
                case TipoNotificacaoApp.ValePedagioCompradoComSucesso: return "Vale pedágio comprado com sucesso";
                case TipoNotificacaoApp.ValePedagioNaoComprado: return "Vale pedágio não comprado - Falha na compra";
                case TipoNotificacaoApp.MotoristaPodeSeguirViagem: return $"Carga entrou na situação \"Em Transporte\"";
                case TipoNotificacaoApp.TratativaDoAtendimento: return $"Tratativa do atendimento";
                case TipoNotificacaoApp.RejeicaoDadosNFeColeta: return $"Rejeição dos dados de NF-e da coleta";
                case TipoNotificacaoApp.Custom: return $"Notificação Customizada";
                default: return string.Empty;
            }
        }
    }
}
