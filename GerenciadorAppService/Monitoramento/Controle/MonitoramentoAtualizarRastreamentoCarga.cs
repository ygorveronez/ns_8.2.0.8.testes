using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SGT.GerenciadorApp.Monitoramento.Controle
{
    public class MonitoramentoAtualizarRastreamentoCarga : IMonitoramento
    {
        private Repositorio.UnitOfWork _unidadeDeTrabalho;
        private Repositorio.Embarcador.Logistica.PosicaoAtual repPosicaoAtual;
        private List<Dominio.Entidades.Embarcador.Logistica.PosicaoAtual> posicoesAtuais;
        private Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento;
        private List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> cargasMonitoradas;
        private Repositorio.Embarcador.GestaoPatio.RastreamentoCarga repRastreamentoCarga;


        private void Inicializar()
        {
            repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(_unidadeDeTrabalho);
            cargasMonitoradas = repMonitoramento.BuscarMonitoramentoIniciado();
            repRastreamentoCarga = new Repositorio.Embarcador.GestaoPatio.RastreamentoCarga(_unidadeDeTrabalho);
            repPosicaoAtual = new Repositorio.Embarcador.Logistica.PosicaoAtual(_unidadeDeTrabalho);

            posicoesAtuais = repPosicaoAtual.BuscarTodos();
        }
        private void Processar()
        {
            try
            {
                foreach (Dominio.Entidades.Embarcador.Logistica.Monitoramento cargaMonitorada in cargasMonitoradas)
                {
                    if (cargaMonitorada?.Carga?.Veiculo == null)
                        continue;

                    var posicaoatual = posicoesAtuais.Where(pos => pos?.Veiculo?.Codigo == cargaMonitorada?.Carga?.Veiculo?.Codigo).FirstOrDefault();

                    if (posicaoatual != null) {
                        Dominio.Entidades.Embarcador.GestaoPatio.RastreamentoCarga rastreamento = repRastreamentoCarga.BuscarPorCarga(cargaMonitorada?.Carga?.Codigo ?? 0);

                        if (rastreamento != null)
                        {
                            rastreamento.Latitude = Convert.ToDecimal(posicaoatual.Latitude);
                            rastreamento.Longitude = Convert.ToDecimal(posicaoatual.Longitude);

                            repRastreamentoCarga.Atualizar(rastreamento);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }
        
        public void Iniciar(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            _unidadeDeTrabalho = unidadeDeTrabalho;

            Inicializar();

            Processar();
        }

    }
}