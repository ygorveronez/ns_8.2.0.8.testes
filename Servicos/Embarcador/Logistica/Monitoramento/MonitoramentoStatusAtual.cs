using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Logistica.Monitoramento
{
    public class ControleStatusAtual
    {

        #region Atributos publicos

        public Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem repRotaFretePontoPassagem;
        public List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> primeiraRotaFretePontoPassagemPorCargas;

        #endregion

        #region Atributos privados

        private Repositorio.UnitOfWork _unidadeDeTrabalho;
        private Repositorio.Embarcador.Logistica.PosicaoAtual repPosicaoAtual;
        private List<Dominio.Entidades.Embarcador.Logistica.PosicaoAtual> posicoesAtuais;
        private Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento;
        private Repositorio.Veiculo repVeiculo;
        private List<Dominio.Entidades.Veiculo> listaVeiculosVinculados;
        private Repositorio.Embarcador.Cargas.Retornos.RetornoCarga repCargaRetorno;
        private Repositorio.Embarcador.Cargas.Carga repCarga;
        private List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> cargasMonitoradas;
        
        private Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoGatilho parametroInicioViagem;
        private Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoGatilho parametroInicoEntrega;
        private Repositorio.Embarcador.Logistica.MonitoramentoEventoGatilho repMonitoramentoEvento;
        private Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega;
        private List <Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargasEntregas;
        private List<Dominio.Entidades.Embarcador.Cargas.Retornos.RetornoCarga> cargasRetono;
        private IList<Dominio.ObjetosDeValor.Embarcador.Carga.VeiculoVinculado> veiculosVinculadosCargas;

        #endregion

        #region Métodos públicos

        public ControleStatusAtual(Repositorio.UnitOfWork unitOfWork)
        {
            _unidadeDeTrabalho = unitOfWork;
        }

        #endregion

        #region Métodos privados

        private void Inicializar()
        {
            repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(_unidadeDeTrabalho);
            cargasMonitoradas = repMonitoramento.BuscarMonitoramentoIniciado();

            if (cargasMonitoradas == null || cargasMonitoradas.Count == 0)
                return;

            repPosicaoAtual = new Repositorio.Embarcador.Logistica.PosicaoAtual(_unidadeDeTrabalho);
            posicoesAtuais = repPosicaoAtual.BuscarTodos();

            repVeiculo = new Repositorio.Veiculo(_unidadeDeTrabalho);

            List<int> codigosCargas = (from cargaMonitorada in cargasMonitoradas select cargaMonitorada.Carga.Codigo).ToList();

            repRotaFretePontoPassagem = new Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem(_unidadeDeTrabalho);
            primeiraRotaFretePontoPassagemPorCargas = repRotaFretePontoPassagem.BuscarPrimeiraRotaFretePorCargas(codigosCargas);

            repCargaRetorno = new Repositorio.Embarcador.Cargas.Retornos.RetornoCarga(_unidadeDeTrabalho);
            cargasRetono = repCargaRetorno.BuscarPorCargas(codigosCargas);

            repMonitoramentoEvento = new Repositorio.Embarcador.Logistica.MonitoramentoEventoGatilho(_unidadeDeTrabalho);

            parametroInicioViagem = repMonitoramentoEvento.BuscarParametorPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMonitoramentoEvento.InicioDeViagem);
            parametroInicoEntrega = repMonitoramentoEvento.BuscarParametorPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMonitoramentoEvento.InicioEntrega);

            repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(_unidadeDeTrabalho);
            cargasEntregas =  repCargaEntrega.BuscarPorCargas(codigosCargas);

            repCarga = new Repositorio.Embarcador.Cargas.Carga(_unidadeDeTrabalho);

            veiculosVinculadosCargas = new List<Dominio.ObjetosDeValor.Embarcador.Carga.VeiculoVinculado>();
            if (codigosCargas.Count > 0)
                veiculosVinculadosCargas = repCarga.BuscarVeiculosVinculadosPorCargas(codigosCargas);

            List<int> codigosVeiculosVinculados = (from veiculosVinculadosCarga in veiculosVinculadosCargas select veiculosVinculadosCarga.CodigoVeiculo).Distinct().ToList();

            listaVeiculosVinculados = repVeiculo.BuscarPorCodigo(codigosVeiculosVinculados);
        }

        private bool ObterEmLoja(Dominio.Entidades.Embarcador.Logistica.Monitoramento cargaMonitorada, Dominio.Entidades.Embarcador.Logistica.PosicaoAtual posicaoAtual)
        {
            if (cargaMonitorada == null)
                return false;

            var cargaEntregas = cargasEntregas.Where(o => o.Carga.Codigo == cargaMonitorada.Carga.Codigo).ToList();


            foreach (var cargaEntrega in cargaEntregas)
            {

                if (cargaEntrega.Cliente == null)
                    continue;

                return Servicos.Embarcador.Logistica.Monitoramento.Monitoramento.ValidarEmRaioOuAreaCliente(cargaEntrega.Cliente, posicaoAtual.Latitude, posicaoAtual.Longitude, parametroInicoEntrega.Raio);
            }

            return false;
        }


        private bool ObterNoCD(Dominio.Entidades.Embarcador.Logistica.Monitoramento cargaMonitorada, Dominio.Entidades.Embarcador.Logistica.PosicaoAtual posicaoAtual)
        {
            if (cargaMonitorada == null)
                return false;


            var rotaFretePontoPassagem = primeiraRotaFretePontoPassagemPorCargas.Where(o => o.CargaRotaFrete.Carga.Codigo == cargaMonitorada.Carga.Codigo).FirstOrDefault();

            if (rotaFretePontoPassagem?.Cliente == null)
                return false;

            return Servicos.Embarcador.Logistica.Monitoramento.Monitoramento.ValidarEmRaioOuAreaCliente(rotaFretePontoPassagem.Cliente, posicaoAtual.Latitude, posicaoAtual.Longitude, parametroInicioViagem.Raio);

        }

        private void AtualizarStatus(Dominio.Entidades.Embarcador.Logistica.Monitoramento cargaMonitorada, Dominio.Entidades.Embarcador.Logistica.PosicaoAtual posicaoAtual) {
            
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPosicao status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPosicao.SemViagem;


            if ((cargaMonitorada != null) && (cargaMonitorada?.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Iniciado))
            {
                status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPosicao.EmViagem;
            }

            posicaoAtual.Status = status;
            repPosicaoAtual.Atualizar(posicaoAtual);
        }

        private void AtualizarVeiculoCarreta(Dominio.Entidades.Embarcador.Logistica.Monitoramento cargaMonitorada, Dominio.Entidades.Embarcador.Logistica.PosicaoAtual posicaoAtual)
        {
            if (cargaMonitorada == null)
                return;
                

            if (posicaoAtual?.Veiculo?.Codigo > 0) {

                var veiculo = posicaoAtual.Veiculo;

                veiculo.Latitude = posicaoAtual.Latitude;
                veiculo.Longitude = posicaoAtual.Longitude;

                repVeiculo.Atualizar(veiculo);

                if(veiculosVinculadosCargas != null && veiculosVinculadosCargas.Count > 0)
                {
                    var veiculosVinculados = veiculosVinculadosCargas.Where(o => cargaMonitorada.Carga != null && o.CodigoCarga == cargaMonitorada.Carga.Codigo).ToList();

                    foreach (var vei in veiculosVinculados)
                    {
                        var veiculoVinculado = listaVeiculosVinculados.Where(o => o.Codigo == vei.CodigoVeiculo).FirstOrDefault();

                        if (veiculoVinculado != null)
                        {
                            veiculoVinculado.Latitude = posicaoAtual.Latitude;
                            veiculoVinculado.Longitude = posicaoAtual.Longitude;
                            repVeiculo.Atualizar(veiculoVinculado);
                        }
                    }

                }
              
           }
        }

        #endregion

    }
}