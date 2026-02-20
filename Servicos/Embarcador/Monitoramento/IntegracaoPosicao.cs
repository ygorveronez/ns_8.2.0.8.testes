using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Monitoramento
{
    public class IntegracaoPosicao
    {
        private Repositorio.Veiculo repVeiculo;
        private List<Dominio.ObjetosDeValor.Embarcador.Veiculos.VeiculoMonitoramento> ListaVeiculos;
        private Repositorio.UnitOfWork _unidadeDeTrabalho;
        private Repositorio.Embarcador.Logistica.PosicaoAtual repPosicaoAtual;
        private List<Dominio.Entidades.Embarcador.Logistica.PosicaoAtual> posicoesAtuais;
        private readonly int TempoMinimoPosicaoEmMinutos;


        public IntegracaoPosicao(Repositorio.UnitOfWork unidadeDeTrabalho, int tempoMinimo)
        {
            TempoMinimoPosicaoEmMinutos = tempoMinimo;
            _unidadeDeTrabalho = unidadeDeTrabalho;
            InicializarVeiculos();
        }
        private void InicializarVeiculos()
        {
            repVeiculo = new Repositorio.Veiculo(_unidadeDeTrabalho);
            ListaVeiculos = repVeiculo.BuscarTodosParaIntegracao();
        }
       
        public bool GravarPosicoes(List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes)
        {
            repPosicaoAtual = new Repositorio.Embarcador.Logistica.PosicaoAtual(_unidadeDeTrabalho);
            posicoesAtuais = repPosicaoAtual.BuscarTodos();
            Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao posicaoLida = null;

            var repPosicao = new Repositorio.Embarcador.Logistica.Posicao(_unidadeDeTrabalho);
            try
            {
                foreach (var pos in posicoes)
                {
                    posicaoLida = pos;
                    if (string.IsNullOrWhiteSpace(pos.IDEquipamento) && string.IsNullOrEmpty(pos.Placa)) 
                        continue;

                    if (pos.Data < DateTime.Now.AddDays(-2))
                        continue;

                    var veiculoObj = !string.IsNullOrWhiteSpace(pos.IDEquipamento) ? ObterVeiculoPorEquipamento(pos.IDEquipamento) : ObterVeiculoPorPlaca(pos.Placa);

                    var veiculo = veiculoObj != null ? new Dominio.Entidades.Veiculo { Codigo = veiculoObj.Codigo, Placa = veiculoObj.Placa } : null;

                    if (veiculo == null)
                        continue;

                    var posicao = new Dominio.Entidades.Embarcador.Logistica.Posicao
                    {
                        Data = pos.Data,
                        DataVeiculo = pos.DataVeiculo,
                        DataCadastro = DateTime.Now,
                        IDEquipamento = pos.IDEquipamento,
                        Veiculo = veiculo,
                        Velocidade = pos.Velocidade,
                        Temperatura = pos.Temperatura,
                        SensorTemperatura = pos.SensorTemperatura?? false,
                        Descricao = pos.Descricao.Length < 100? pos.Descricao : pos.Descricao.Substring(0, 99),
                        Latitude = pos.Latitude,
                        Longitude = pos.Longitude,
                        Ignicao = pos.Ignicao
                    };

                    repPosicao.Inserir(posicao);

                }

                return true;
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro("Erro ao ler posicao" + posicaoLida?.ID??"");
                Servicos.Log.TratarErro(excecao);
                return false;
            }
        }
       
        private Dominio.ObjetosDeValor.Embarcador.Veiculos.VeiculoMonitoramento ObterVeiculoPorPlaca(string Placa)
        {
            return ListaVeiculos.Where(s => s?.Placa == Placa).FirstOrDefault();
        }
        private Dominio.ObjetosDeValor.Embarcador.Veiculos.VeiculoMonitoramento ObterVeiculoPorEquipamento(string Equipamento)
        {
            return ListaVeiculos.Where(s => s.NumeroEquipamentoRastreador == Equipamento).FirstOrDefault();
        }
       
    }
}


