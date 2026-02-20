using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoGestaoDevolucao
    {
        NaoDefinido = 0,
        Permuta = 1,
        Coleta = 2,
        Descarte = 3,
        Agendamento = 4,
        PosEntrega = 5,
        PermutaPallet = 6,
        Simplificado = 7,
    }

    public static class TipoGestaoDevolucaoHelper
    {
        public static string ObterDescricao(this TipoGestaoDevolucao TipoGestaoDevolucao)
        {
            switch (TipoGestaoDevolucao)
            {
                case TipoGestaoDevolucao.Permuta: return "Permuta";
                case TipoGestaoDevolucao.Coleta: return "Coleta";
                case TipoGestaoDevolucao.Descarte: return "Descarte";
                case TipoGestaoDevolucao.Agendamento: return "Agendamento";
                case TipoGestaoDevolucao.PosEntrega: return "Pós Entrega";
                case TipoGestaoDevolucao.PermutaPallet: return "Permuta";
                case TipoGestaoDevolucao.Simplificado: return "Simplificado";
                case TipoGestaoDevolucao.NaoDefinido:
                default: return "Não Definido";
            }
        }
        public static List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaGestaoDevolucao> ObterEtapasDevolucao(this TipoGestaoDevolucao TipoGestaoDevolucao, TipoNotasGestaoDevolucao tipoNotas)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaGestaoDevolucao> etapas = new();

            void AdicionarEtapaPallet()
            {
                if (tipoNotas == TipoNotasGestaoDevolucao.Pallet)
                    etapas.Add(EtapaGestaoDevolucao.DocumentacaoEntradaFiscal);
            }

            switch (TipoGestaoDevolucao)
            {
                case TipoGestaoDevolucao.Permuta:
                    etapas.Add(EtapaGestaoDevolucao.OrdemeRemessa);
                    etapas.Add(EtapaGestaoDevolucao.GeracaoOcorrenciaDebito);
                    AdicionarEtapaPallet();
                    etapas.Add(EtapaGestaoDevolucao.GeracaoLaudo);
                    etapas.Add(EtapaGestaoDevolucao.AprovacaoLaudo);
                    etapas.Add(EtapaGestaoDevolucao.IntegracaoLaudo);
                    break;

                case TipoGestaoDevolucao.Coleta:
                    etapas.Add(EtapaGestaoDevolucao.DefinicaoLocalColeta);
                    etapas.Add(EtapaGestaoDevolucao.GeracaoCargaDevolucao);
                    AdicionarEtapaPallet();
                    etapas.Add(EtapaGestaoDevolucao.AgendamentoParaDescarga);
                    etapas.Add(EtapaGestaoDevolucao.OrdemeRemessa);
                    etapas.Add(EtapaGestaoDevolucao.Monitoramento);
                    etapas.Add(EtapaGestaoDevolucao.GeracaoLaudo);
                    etapas.Add(EtapaGestaoDevolucao.AprovacaoLaudo);
                    etapas.Add(EtapaGestaoDevolucao.IntegracaoLaudo);
                    break;

                case TipoGestaoDevolucao.Descarte:
                    etapas.Add(EtapaGestaoDevolucao.OrdemeRemessa);
                    etapas.Add(EtapaGestaoDevolucao.GestaoCustoContabil);
                    AdicionarEtapaPallet();
                    etapas.Add(EtapaGestaoDevolucao.GeracaoLaudo);
                    etapas.Add(EtapaGestaoDevolucao.AprovacaoLaudo);
                    etapas.Add(EtapaGestaoDevolucao.IntegracaoLaudo);
                    break;

                case TipoGestaoDevolucao.Agendamento:
                    etapas.Add(EtapaGestaoDevolucao.Agendamento);
                    etapas.Add(EtapaGestaoDevolucao.AprovacaoDataDescarga);
                    etapas.Add(EtapaGestaoDevolucao.OrdemeRemessa);
                    etapas.Add(EtapaGestaoDevolucao.GeracaoCargaDevolucao);
                    AdicionarEtapaPallet();
                    etapas.Add(EtapaGestaoDevolucao.Monitoramento);
                    etapas.Add(EtapaGestaoDevolucao.GeracaoLaudo);
                    etapas.Add(EtapaGestaoDevolucao.AprovacaoLaudo);
                    etapas.Add(EtapaGestaoDevolucao.IntegracaoLaudo);
                    break;

                case TipoGestaoDevolucao.NaoDefinido:
                    etapas.Add(EtapaGestaoDevolucao.GestaoDeDevolucao);
                    etapas.Add(EtapaGestaoDevolucao.DefinicaoTipoDevolucao);
                    etapas.Add(EtapaGestaoDevolucao.AprovacaoTipoDevolucao);
                    break;

                case TipoGestaoDevolucao.PosEntrega:
                    etapas.Add(EtapaGestaoDevolucao.GestaoDeDevolucao);
                    AdicionarEtapaPallet();
                    etapas.Add(EtapaGestaoDevolucao.AprovacaoCenarioPosEntrega);
                    etapas.Add(EtapaGestaoDevolucao.CenarioPosEntrega);
                    break;

                case TipoGestaoDevolucao.PermutaPallet:
                    AdicionarEtapaPallet();
                    etapas.Add(EtapaGestaoDevolucao.OrdemeRemessa);
                    etapas.Add(EtapaGestaoDevolucao.RegistroDocumentosPallet);
                    break;

                case TipoGestaoDevolucao.Simplificado:
                    etapas.Add(EtapaGestaoDevolucao.GestaoDeDevolucao);
                    etapas.Add(EtapaGestaoDevolucao.GeracaoLaudo);
                    etapas.Add(EtapaGestaoDevolucao.AprovacaoLaudo);
                    etapas.Add(EtapaGestaoDevolucao.IntegracaoLaudo);
                    break;

                default: break;
            }
            return etapas;
        }
    }
}