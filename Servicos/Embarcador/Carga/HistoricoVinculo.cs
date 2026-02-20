using Dominio.Entidades.Embarcador.Logistica;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Carga
{
    public class HistoricoVinculo : ServicoBase
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Cargas.HistoricoVinculo _repHistorico;

        #endregion

        #region Construtores

        public HistoricoVinculo(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork)
        {
            _repHistorico = new Repositorio.Embarcador.Cargas.HistoricoVinculo(unitOfWork);
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Insere um histórico de vínculo de motorista/veiculo na carga/pedido.
        /// </summary>
        /// <param name="erros">String de erros.</param>
        /// <returns>Retorna verdadeiro se a inserção for bem-sucedida.</returns>
        public bool InserirHistoricoVinculo(Repositorio.UnitOfWork unitOfWork, ref string erros, LocalVinculo localVinculo, Dominio.Entidades.Veiculo veiculoTracao = null, ICollection<Dominio.Entidades.Veiculo> veiculoReboques = null,
                                            ICollection<Dominio.Entidades.Usuario> motoristas = null, DateTime? dataVinculo = null, DateTime? dataDesvinculo = null, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = null,
                                            Dominio.Entidades.Embarcador.Cargas.Carga carga = null, FilaCarregamentoVeiculo filaCarregamento = null, string observacao = "")
        {
            erros = string.Empty;

            # region  Validação dos parâmetros
            if ((motoristas == null || !motoristas.Any()) && veiculoTracao == null && (veiculoReboques == null || !veiculoReboques.Any()))
                erros += "É necessário ter um veículo de tração, reboque ou motorista vinculado; ";

            if (dataVinculo == null && dataDesvinculo == null)
                erros += "É necessário ter uma data de vinculo ou desvinculo vinculada; ";

            if (pedido == null && carga == null && filaCarregamento == null)
                erros += "É necessário ter um pedido, carga ou fila de carregamento vinculado; ";

            if (erros != string.Empty)
                return false;

            #endregion

            CriarObservacaoHistorico(localVinculo, veiculoTracao, veiculoReboques, motoristas, dataVinculo, dataDesvinculo, pedido, carga, filaCarregamento, ref observacao);

            try
            {
                // Verifica se precisa adicionar uma linha de remoção de vinculo
                // AdicionarRemocaoVinculoExistente(localVinculo, veiculoTracao, veiculoReboques, motoristas, dataVinculo, pedido, carga, filaCarregamento, repHistorco);

                if (_repHistorico.ExisteRegistroIgual(veiculoTracao, veiculoReboques, motoristas, dataVinculo, dataDesvinculo, pedido, carga, filaCarregamento))
                {
                    erros += "Já existe um vinculo ativo com os mesmos dados";
                    return false;
                }

                // Insere o novo vinculo
                var novaListaVeiculoReboques = veiculoReboques?.ToList();
                var novaListaMotoristas = motoristas?.ToList();

                var historicoVinculo = new Dominio.Entidades.Embarcador.Cargas.HistoricoVinculo
                {
                    VeiculoTracao = veiculoTracao,
                    VeiculoReboques = novaListaVeiculoReboques,
                    Motoristas = novaListaMotoristas,
                    LocalVinculo = localVinculo,
                    DataHoraVinculo = dataVinculo,
                    DataHoraDesvinculo = dataDesvinculo,
                    Pedido = pedido,
                    Carga = carga,
                    FilaCarregamento = filaCarregamento,
                    Observacao = observacao
                };


                _repHistorico.Inserir(historicoVinculo);

            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);
                erros = ex.Message;
                return false;
            }

            return true;
        }

        #endregion

        #region Métodos Privados

        /*
        //To-do - Criar um metodo para buscar se esta subistituindo um vinculo e adicionar a descricao de "removido vinculo .. " antes de adicionar o novo vinculo
        private static void AdicionarRemocaoVinculoExistente(LocalVinculo localVinculo, Dominio.Entidades.Veiculo veiculoTracao, ICollection<Dominio.Entidades.Veiculo> veiculoReboques, ICollection<Usuario> motoristas, DateTime? dataVinculo, 
                                                             Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Carga carga, FilaCarregamentoVeiculo filaCarregamento, Repositorio.Embarcador.Cargas.HistoricoVinculo repositorioHistoricoVinculo)
        {
            var vinculoExistente = repositorioHistoricoVinculo.BuscarVinculoAtivo(
                veiculoTracao, veiculoReboques, motoristas, pedido, carga, filaCarregamento
            );

            if (vinculoExistente != null)
            {
                var registroRemocao = vinculoExistente;
                registroRemocao.DataVinculo = null;
                registroRemocao.DataHoraDesvinculo = DateTime.Now;

                var observacao = string.Empty;
                CriarObservacaoHistorico(registroRemocao.localVinculo, registroRemocao.veiculoTracao, registroRemocao.veiculoReboques,
                    registroRemocao.motoristas, null, registroRemocao.dataDesvinculo, registroRemocao.pedido, registroRemocao.carga,
                    registroRemocao.filaCarregamento, ref observacao);

                vinculoExistente.Observacao = observacao;

                // Inserir um novo histórico de desvinculação
                repositorioHistoricoVinculo.Inserir(registroRemocao);
            }
        }
        */

        private static void CriarObservacaoHistorico(LocalVinculo localVinculo, Dominio.Entidades.Veiculo veiculoTracao, ICollection<Dominio.Entidades.Veiculo> veiculoReboques, ICollection<Dominio.Entidades.Usuario> motoristas, DateTime? dataVinculo, DateTime? dataDesvinculo,
                                                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.Entidades.Embarcador.Cargas.Carga carga, FilaCarregamentoVeiculo filaCarregamento, ref string observacaoPrevia)
        {
            string observacao = $"({LocalVinculoHelper.ObterDescricao(localVinculo)}) - {(dataVinculo != null ? dataVinculo : dataDesvinculo)} / ";

            if (!string.IsNullOrWhiteSpace(observacaoPrevia))
            {
                observacao += observacaoPrevia + " Restando ";
            }
            else
            {
                if (dataVinculo != null)
                    observacao += "Adicionado ";
                else if (dataDesvinculo != null)
                    observacao += "Removido ";

            }

            if (veiculoTracao != null)
                observacao += "a tração " + veiculoTracao.Placa + ", ";

            if (veiculoReboques != null && veiculoReboques.Any())
            {
                observacao += veiculoReboques.Count > 1
                    ? "os reboques " + string.Join(", ", veiculoReboques.Select(v => v.Placa)) + ", "
                    : "o reboque " + veiculoReboques.First().Placa + ", ";
            }

            if (motoristas != null && motoristas.Any())
            {
                observacao += motoristas.Count > 1
                    ? "os motoristas " + string.Join(", ", motoristas.Select(m => m.Nome)) + ", "
                    : "o motorista " + motoristas.First().Nome + " ";
            }

            if (carga != null)
                observacao += "na carga " + carga.CodigoCargaEmbarcador + " ";

            if (pedido != null)
                observacao += "no pedido " + (string.IsNullOrWhiteSpace(pedido.NumeroPedidoEmbarcador) ? pedido.Numero : pedido.NumeroPedidoEmbarcador) + " ";

            if (filaCarregamento != null)
                observacao += "na fila de carregamento " + filaCarregamento.Codigo + " ";

            observacaoPrevia = observacao;

        }
        #endregion
    }
}
