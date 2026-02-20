using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SGT.GerenciadorApp.Monitoramento.Controle
{
    public class MonitoramentoControleStatusAtual : IMonitoramento
    {
        private Repositorio.UnitOfWork _unidadeDeTrabalho;
        private Repositorio.Embarcador.Logistica.PosicaoAtual repPosicaoAtual;
        private List<Dominio.Entidades.Embarcador.Logistica.PosicaoAtual> posicoesAtuais;
        private Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao;
        private Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao;
        private Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento;
        private Repositorio.Veiculo repVeiculo;
        private Repositorio.Embarcador.Cargas.Retornos.RetornoCarga repCargaRetorno;
        private List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> cargasMonitoradas;
       
        private void Inicializar()
        {
            repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(_unidadeDeTrabalho);
            cargasMonitoradas = repMonitoramento.BuscarMonitoramentoIniciado();

            repPosicaoAtual = new Repositorio.Embarcador.Logistica.PosicaoAtual(_unidadeDeTrabalho);
            posicoesAtuais = repPosicaoAtual.BuscarTodos();

            repVeiculo = new Repositorio.Veiculo(_unidadeDeTrabalho);

            repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(_unidadeDeTrabalho);
            configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();

            repCargaRetorno = new Repositorio.Embarcador.Cargas.Retornos.RetornoCarga(_unidadeDeTrabalho);
        }
        private void AtualizarStatus(Dominio.Entidades.Embarcador.Logistica.Monitoramento cargaMonitorada, Dominio.Entidades.Embarcador.Logistica.PosicaoAtual posicaoAtual) {
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPosicao status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPosicao.SemViagem;

            if ((cargaMonitorada != null) && (cargaMonitorada?.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Iniciado))
            {
                status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPosicao.EmViagem;

                if (cargaMonitorada?.Carga != null)
                {
                    var cargarRetorno = repCargaRetorno.BuscarPorCarga(cargaMonitorada.Carga.Codigo);

                    if (cargarRetorno != null)
                        status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPosicao.Retornando;
                }
            }

            if (posicaoAtual.DataVeiculo.AddMinutes(60) < DateTime.Now)
                status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPosicao.SemPosicao;


            SGT.GerenciadorApp.Monitoramento.MonitoramentoUtils.GravarLogTracking($"Atualizando status do veículo {posicaoAtual?.Veiculo?.Placa??"ID"+posicaoAtual?.IDEquipamento} para {status.ToString()}", this.GetType().Name);

            posicaoAtual.Status = status;
            repPosicaoAtual.Atualizar(posicaoAtual);
        }
        private void AtualizarVeiculoCarreta(Dominio.Entidades.Embarcador.Logistica.Monitoramento cargaMonitorada, Dominio.Entidades.Embarcador.Logistica.PosicaoAtual posicaoAtual)
        {
            if (cargaMonitorada == null)
                return;
                

            if (posicaoAtual?.Veiculo?.Codigo > 0) {

                var veiculo = repVeiculo.BuscarPorCodigo(posicaoAtual.Veiculo.Codigo);
                veiculo.Latitude = posicaoAtual.Latitude;
                veiculo.Longitude = posicaoAtual.Longitude;

                repVeiculo.Atualizar(veiculo);

                foreach (var veiculoVinculado in cargaMonitorada?.Carga?.VeiculosVinculados)
                {
                    veiculoVinculado.Latitude = posicaoAtual.Latitude;
                    veiculoVinculado.Longitude = posicaoAtual.Longitude;
                    repVeiculo.Atualizar(veiculoVinculado);
                }
           }
        }
        private void Processar()
        {
            try
            {
                SGT.GerenciadorApp.Monitoramento.MonitoramentoUtils.GravarLogTracking("Processando posições atuais", this.GetType().Name);

                foreach (var posicaoAtual in posicoesAtuais)
                {
                    var cargaMonitorada = cargasMonitoradas.Where(car => car?.Carga?.Veiculo?.Codigo == posicaoAtual.Veiculo?.Codigo).FirstOrDefault();

                    AtualizarStatus(cargaMonitorada, posicaoAtual);
                    AtualizarVeiculoCarreta(cargaMonitorada, posicaoAtual);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }
        public void Iniciar(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            SGT.GerenciadorApp.Monitoramento.MonitoramentoUtils.GravarLogTracking("Inicio", this.GetType().Name);
            _unidadeDeTrabalho = unidadeDeTrabalho;

            Inicializar();

            Processar();

            SGT.GerenciadorApp.Monitoramento.MonitoramentoUtils.GravarLogTracking("Fim\r\n", this.GetType().Name);

        }

    }
}