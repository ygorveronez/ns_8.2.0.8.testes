using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;


namespace SGT.GerenciadorApp.Monitoramento.Integracao
{
    public class IntegracaoPosicao
    {
        private Repositorio.Veiculo repVeiculo;
        private List<Dominio.Entidades.Veiculo> ListaVeiculos;
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
            ListaVeiculos = repVeiculo.BuscarTodosVeiculos();
        }
       
        private Dominio.Entidades.Embarcador.Logistica.PosicaoAtual InserirNovaPosicaoAtual(Dominio.Entidades.Embarcador.Logistica.Posicao posicao)
        {
            var novaPosicao = new Dominio.Entidades.Embarcador.Logistica.PosicaoAtual
            {
                Data = posicao.Data,
                DataVeiculo = posicao.Data,
                DataCadastro = DateTime.Now,
                IDEquipamento = posicao.IDEquipamento,
                Velocidade = posicao.Velocidade,
                Temperatura = posicao.Temperatura,
                SensorTemperatura = posicao.SensorTemperatura,
                Descricao = posicao.Descricao,
                Latitude = posicao.Latitude,
                Longitude = posicao.Longitude,
                Ignicao = posicao.Ignicao,
                Veiculo = posicao.Veiculo,
                Posicao = posicao
            };

            repPosicaoAtual.Inserir(novaPosicao);

            return novaPosicao;
        }
        private void AtualizarDadosPosicaoAtual(Dominio.Entidades.Embarcador.Logistica.PosicaoAtual posAtual, Dominio.Entidades.Embarcador.Logistica.Posicao novaPosicao)
        {
            posAtual.Data = novaPosicao.Data;
            posAtual.DataVeiculo = novaPosicao.DataVeiculo;
            posAtual.DataCadastro = DateTime.Now;
            posAtual.Descricao = novaPosicao.Descricao;
            posAtual.IDEquipamento = novaPosicao.IDEquipamento;
            posAtual.Latitude = novaPosicao.Latitude;
            posAtual.Longitude = novaPosicao.Longitude;
            posAtual.Velocidade = novaPosicao.Velocidade;
            posAtual.Temperatura = novaPosicao.Temperatura;
            posAtual.Ignicao = novaPosicao.Ignicao;
            posAtual.Veiculo = novaPosicao.Veiculo;
            posAtual.SensorTemperatura = novaPosicao.SensorTemperatura;
            posAtual.Posicao = novaPosicao;
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
                    if (pos.IDEquipamento == 0 && string.IsNullOrEmpty(pos.Placa)) 
                        continue;

                    if (pos.Data < DateTime.Now.AddDays(-2))
                        continue;

                    Dominio.Entidades.Embarcador.Logistica.PosicaoAtual posicaoAtual;

                    if (pos.IDEquipamento > 0)
                        posicaoAtual = posicoesAtuais.Where(item => item.IDEquipamento == pos.IDEquipamento).FirstOrDefault();
                    else
                        posicaoAtual = posicoesAtuais.Where(item => item?.Veiculo?.Placa == pos.Placa).FirstOrDefault();


                    if ((posicaoAtual != null) && posicaoAtual.DataVeiculo.AddMinutes(TempoMinimoPosicaoEmMinutos) > pos.Data)
                        continue;

                    var Veiculo = posicaoAtual?.Veiculo ?? (pos.IDEquipamento > 0 ? ObterVeiculoPorEquipamento(pos.IDEquipamento) : ObterVeiculoPorPlaca(pos.Placa));

                    if (Veiculo == null)
                        continue;

                    var posicao = new Dominio.Entidades.Embarcador.Logistica.Posicao
                    {
                        Data = pos.Data,
                        DataVeiculo = pos.DataVeiculo,
                        DataCadastro = DateTime.Now,
                        IDEquipamento = pos.IDEquipamento,
                        Veiculo = Veiculo ?? null,
                        Velocidade = pos.Velocidade,
                        Temperatura = pos.Temperatura,
                        SensorTemperatura = pos.SensorTemperatura?? false,
                        Descricao = pos.Descricao.Length < 100? pos.Descricao : pos.Descricao.Substring(0, 99),
                        Latitude = pos.Latitude,
                        Longitude = pos.Longitude,
                        Ignicao = pos.Ignicao
                    };

                    repPosicao.Inserir(posicao);

                    if (posicaoAtual == null)
                        posicoesAtuais.Add(InserirNovaPosicaoAtual(posicao));
                    else
                        AtualizarDadosPosicaoAtual(posicaoAtual, posicao);
                }

                GravarPosicaoAtual();

                return true;
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro("Erro ao ler posicao" + posicaoLida?.ID??"");
                Servicos.Log.TratarErro(excecao);
                return false;
            }
        }
        private void GravarPosicaoAtual()
        {
            foreach (var posAtual in posicoesAtuais)
            {
                repPosicaoAtual.Atualizar(posAtual);
            }
        }
       
        private Dominio.Entidades.Veiculo ObterVeiculoPorPlaca(string Placa)
        {
            return ListaVeiculos.Where(s => s?.Placa == Placa).FirstOrDefault();
        }
        private Dominio.Entidades.Veiculo ObterVeiculoPorEquipamento(int Equipamento)
        {
            return ListaVeiculos.Where(s => s.NumeroEquipamentoRastreador == Equipamento.ToString()).FirstOrDefault();
        }
       
    }
}


