using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.WebService;

namespace Servicos.WebService.Monitoramento
{
    public class Monitoramento : ServicoWebServiceBase
    {
        #region Propriedades Privadas

        Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public Monitoramento(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { _unitOfWork = unitOfWork; }

        #endregion

        #region Métodos Públicos

        public Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Monitoramento.Rastreadores> ConsultaPosicionamentoStatusDispositivo(Dominio.ObjetosDeValor.WebService.Monitoramento.ConsultaPosicionamentoVeiculo consultaPosicionamentoVeiculo)
        {
            Servicos.Log.TratarErro($"ConsultaPosicionamentoStatusDispositivo: {(consultaPosicionamentoVeiculo != null ? Newtonsoft.Json.JsonConvert.SerializeObject(consultaPosicionamentoVeiculo) : string.Empty)}");

            List<Dominio.ObjetosDeValor.WebService.Monitoramento.PosicionamentoVeiculo> retorno = new List<Dominio.ObjetosDeValor.WebService.Monitoramento.PosicionamentoVeiculo>();

            string mensagem = string.Empty;

            Repositorio.Embarcador.Logistica.PosicaoAtual repositorioPosicaoAtual = new Repositorio.Embarcador.Logistica.PosicaoAtual(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMotorista repositorioCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(_unitOfWork);
            Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(_unitOfWork);
            Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(_unitOfWork);
            Repositorio.Embarcador.Veiculos.VeiculoMotorista repositorioVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(_unitOfWork);
            Repositorio.Embarcador.Logistica.Monitoramento repositorioMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(_unitOfWork);

            if (!string.IsNullOrWhiteSpace(consultaPosicionamentoVeiculo.CPF) && string.IsNullOrWhiteSpace(consultaPosicionamentoVeiculo.Placa))
            {
                //apenas validar se motorista possui a flag..
                Dominio.Entidades.Usuario motoristaValidar = !string.IsNullOrWhiteSpace(consultaPosicionamentoVeiculo.CPF) ? repositorioUsuario.BuscarPorCPF(consultaPosicionamentoVeiculo.CPF) : null;
                if (motoristaValidar != null && motoristaValidar.CodigoMobile > 0)
                {
                    retorno.Add(ConverterObjetoPosicionamentoConsultaMotorista(motoristaValidar));

                    Dominio.ObjetosDeValor.WebService.Monitoramento.Rastreadores rastreadoresMotoristaValidar = new Dominio.ObjetosDeValor.WebService.Monitoramento.Rastreadores()
                    {
                        PosicionamentoVeiculos = retorno
                    };

                    Servicos.Log.TratarErro($"Retorno apenas validar Motorista: {(rastreadoresMotoristaValidar != null ? Newtonsoft.Json.JsonConvert.SerializeObject(rastreadoresMotoristaValidar) : string.Empty)}");
                    return Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Monitoramento.Rastreadores>.CriarRetornoSucesso(rastreadoresMotoristaValidar);
                }
                else
                {
                    Servicos.Log.TratarErro($"Retorno: Motorista sem cadastro no multiMobile.");
                    return Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Monitoramento.Rastreadores>.CriarRetornoDadosInvalidos("Motorista sem cadastro no multiMobile.", Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos);
                }
            }

            if (string.IsNullOrWhiteSpace(consultaPosicionamentoVeiculo.CPF) && string.IsNullOrWhiteSpace(consultaPosicionamentoVeiculo.Placa) && string.IsNullOrWhiteSpace(consultaPosicionamentoVeiculo.Transporte))
            {
                Servicos.Log.TratarErro($"Retorno: Sem Parâmetros informados.");
                return Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Monitoramento.Rastreadores>.CriarRetornoDadosInvalidos("Sem Parâmetros informados.", Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos);
            }

            Dominio.Entidades.Embarcador.Cargas.Carga carga = !string.IsNullOrWhiteSpace(consultaPosicionamentoVeiculo.Transporte) ? repositorioCarga.BuscarPorCodigoEmbarcador(consultaPosicionamentoVeiculo.Transporte) : null;
            Dominio.Entidades.Veiculo veiculo = !string.IsNullOrWhiteSpace(consultaPosicionamentoVeiculo.Placa) ? repositorioVeiculo.BuscarPorPlacaMaisRecente(consultaPosicionamentoVeiculo.Placa) : null;
            Dominio.Entidades.Usuario motorista = !string.IsNullOrWhiteSpace(consultaPosicionamentoVeiculo.CPF) ? repositorioUsuario.BuscarPorCPF(consultaPosicionamentoVeiculo.CPF) : null;

            if (carga == null && veiculo == null && motorista == null)
            {
                Servicos.Log.TratarErro($"Retorno: Não foi possível localizar os dados com estes parâmetros.");
                return Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Monitoramento.Rastreadores>.CriarRetornoDadosInvalidos("Não foi possível localizar os dados com estes parâmetros.", Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos);
            }

            if (carga != null && veiculo != null && motorista != null)
            {
                Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento = repositorioMonitoramento.BuscarPorCargaNaoFinalizado(carga.Codigo);

                if (monitoramento == null)
                {
                    Servicos.Log.TratarErro($"Retorno: Não foi encontrado monitoramento em andamento para Carga: {carga.CodigoCargaEmbarcador}");
                    return Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Monitoramento.Rastreadores>.CriarRetornoDadosInvalidos($"Não foi encontrado monitoramento em andamento para Carga: {carga.CodigoCargaEmbarcador}", Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos);
                }

                if (carga.Veiculo.Codigo != veiculo.Codigo)
                {
                    Servicos.Log.TratarErro($"Retorno: Veículo da Carga não é igual ao veiculo informado para busca, favor verificar. Carga {carga.CodigoCargaEmbarcador} Placa: {veiculo.Placa}");
                    return Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Monitoramento.Rastreadores>.CriarRetornoDadosInvalidos($"Veículo da Carga não é igual ao veiculo informado para busca, favor verificar. Carga {carga.CodigoCargaEmbarcador} Placa: {veiculo.Placa}", Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos);
                }

                List<Dominio.Entidades.Embarcador.Cargas.CargaMotorista> cargaMotoristas = repositorioCargaMotorista.BuscarPorCarga(carga.Codigo);
                if (cargaMotoristas.Any(obj => obj.Motorista.Codigo == motorista.Codigo))
                {
                    Dominio.Entidades.Embarcador.Logistica.PosicaoAtual posicaoAtual = repositorioPosicaoAtual.BuscarPorVeiculo(veiculo.Codigo);

                    if (posicaoAtual != null)
                        retorno.Add(ConverterObjetoPosicionamentoVeiculo(posicaoAtual, carga));
                    else
                        mensagem = "Não foi encontrado a última posição deste Veículo com os Parametros informados, favor verificar.";
                }
                else
                {
                    Servicos.Log.TratarErro($"Retorno: Carga não possui o Motorista enviado para busca, favor verificar. {motorista.Nome} - {motorista.CPF} Carga: {carga.CodigoCargaEmbarcador}");
                    return Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Monitoramento.Rastreadores>.CriarRetornoDadosInvalidos($"Carga não possui o Motorista enviado para busca, favor verificar. {motorista.Nome} - {motorista.CPF} Carga: {carga.CodigoCargaEmbarcador}", Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos);
                }
            }
            else if (carga != null && veiculo != null && motorista == null)
            {
                Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento = repositorioMonitoramento.BuscarPorCargaNaoFinalizado(carga.Codigo);

                if (monitoramento == null)
                {
                    Servicos.Log.TratarErro($"Retorno: Não foi encontrado monitoramento em andamento para Carga: {carga.CodigoCargaEmbarcador}");
                    return Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Monitoramento.Rastreadores>.CriarRetornoDadosInvalidos($"Não foi encontrado monitoramento em andamento para Carga: {carga.CodigoCargaEmbarcador}", Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos);
                }

                if (carga.Veiculo.Codigo != veiculo.Codigo)
                {
                    Servicos.Log.TratarErro($"Retorno: Veículo da Carga não é igual ao veiculo informado para busca, favor verificar. Carga {carga.CodigoCargaEmbarcador} Placa: {veiculo.Placa}");
                    return Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Monitoramento.Rastreadores>.CriarRetornoDadosInvalidos($"Veículo da Carga não é igual ao veiculo informado para busca, favor verificar. Carga {carga.CodigoCargaEmbarcador} Placa: {veiculo.Placa}", Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos);
                }

                Dominio.Entidades.Embarcador.Logistica.PosicaoAtual posicaoAtual = repositorioPosicaoAtual.BuscarPorVeiculo(veiculo.Codigo);

                if (posicaoAtual != null)
                    retorno.Add(ConverterObjetoPosicionamentoVeiculo(posicaoAtual, carga));
                else
                    mensagem = "Não foi encontrado a última posição deste Veículo com os Parametros informados, favor verificar.";
            }
            else if (carga != null && motorista != null && veiculo == null)
            {
                Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento = repositorioMonitoramento.BuscarPorCargaNaoFinalizado(carga.Codigo);

                if (monitoramento == null)
                {
                    Servicos.Log.TratarErro($"Retorno: Não foi encontrado monitoramento em andamento para Carga: {carga.CodigoCargaEmbarcador}");
                    return Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Monitoramento.Rastreadores>.CriarRetornoDadosInvalidos($"Não foi encontrado monitoramento em andamento para Carga: {carga.CodigoCargaEmbarcador}", Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos);
                }

                List<Dominio.Entidades.Embarcador.Cargas.CargaMotorista> cargaMotoristas = repositorioCargaMotorista.BuscarPorCarga(carga.Codigo);
                if (cargaMotoristas.Any(obj => obj.Motorista.Codigo == motorista.Codigo))
                {
                    Dominio.Entidades.Embarcador.Logistica.PosicaoAtual posicaoAtual = repositorioPosicaoAtual.BuscarPorVeiculo(carga?.Veiculo?.Codigo ?? 0);

                    if (posicaoAtual != null)
                        retorno.Add(ConverterObjetoPosicionamentoVeiculo(posicaoAtual, carga));
                    else
                        mensagem = "Não foi encontrado a última posição deste Veículo com os Parametros informados, favor verificar.";
                }
                else
                {
                    Servicos.Log.TratarErro($"Retorno: Carga não possui o Motorista enviado para busca, favor verificar. {motorista.Nome} - {motorista.CPF} Carga: {carga.CodigoCargaEmbarcador}");
                    return Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Monitoramento.Rastreadores>.CriarRetornoDadosInvalidos($"Carga não possui o Motorista enviado para busca, favor verificar. {motorista.Nome} - {motorista.CPF} Carga: {carga.CodigoCargaEmbarcador}", Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos);
                }
            }
            else if (veiculo != null && motorista != null && carga == null)
            {
                if (!repositorioMonitoramento.ExisteMonitoramentoInciadoPorVeiculoEMotorista(veiculo, motorista))
                {
                    Servicos.Log.TratarErro($"Retorno: Não foi encontrado a última posição deste Veículo e Motorista com os Parametros informados, favor verificar. Motorista: {motorista.Nome} - {motorista.CPF} Veiculo: {veiculo.Placa}");
                    return Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Monitoramento.Rastreadores>.CriarRetornoDadosInvalidos($"Não foi encontrado a última posição deste Veículo e Motorista com os Parametros informados, favor verificar. Motorista: {motorista.Nome} - {motorista.CPF} Veiculo: {veiculo.Placa}", Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos);
                }

                Dominio.Entidades.Embarcador.Logistica.PosicaoAtual posicaoAtual = repositorioPosicaoAtual.BuscarPorVeiculo(veiculo.Codigo);

                if (posicaoAtual != null)
                    retorno.Add(ConverterObjetoPosicionamentoVeiculo(posicaoAtual, carga));
                else
                    mensagem = "Não foi encontrado a última posição deste Veículo com os Parametros informados, favor verificar.";
            }
            else if (carga != null && veiculo == null && motorista == null)
            {
                Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento = repositorioMonitoramento.BuscarAtivoPorCarga(carga.Codigo);

                if (monitoramento == null)
                {
                    Servicos.Log.TratarErro($"Retorno: Não foi encontrado monitoramento em andamento para Carga: {carga.CodigoCargaEmbarcador}");
                    return Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Monitoramento.Rastreadores>.CriarRetornoDadosInvalidos($"Não foi encontrado monitoramento em andamento para Carga: {carga.CodigoCargaEmbarcador}", Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos);
                }

                Dominio.Entidades.Embarcador.Logistica.PosicaoAtual posicaoAtual = repositorioPosicaoAtual.BuscarPorVeiculo(monitoramento.Veiculo?.Codigo ?? 0);

                if (posicaoAtual != null)
                    retorno.Add(ConverterObjetoPosicionamentoVeiculo(posicaoAtual, carga));
                else
                    mensagem = "Não foi encontrado a última posição deste Veículo com os Parametros informados, favor verificar.";
            }
            else if (veiculo != null && carga == null && motorista == null)
            {
                Dominio.Entidades.Embarcador.Logistica.PosicaoAtual posicaoAtual = repositorioPosicaoAtual.BuscarPorVeiculo(veiculo.Codigo);

                if (posicaoAtual != null)
                    retorno.Add(ConverterObjetoPosicionamentoVeiculo(posicaoAtual, carga));
                else
                    mensagem = "Não foi encontrado a última posição deste Veículo com os Parametros informados, favor verificar.";
            }
            else if (motorista != null && veiculo == null && carga == null)
            {
                List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> listaMonitoramento = repositorioMonitoramento.BuscarMonitoramentoEmAbertoPorMotorista(motorista);

                if (listaMonitoramento?.Count == 0)
                {
                    Servicos.Log.TratarErro($"Retorno: Não foi encontrado a última posição deste Veículo com os Parametros informados, favor verificar. Motorista: {motorista.Nome} - {motorista.CPF}");
                    return Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Monitoramento.Rastreadores>.CriarRetornoDadosInvalidos($"Não foi encontrado a última posição deste Veículo com os Parametros informados, favor verificar. Motorista: {motorista.Nome} - {motorista.CPF}", Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos);
                }

                foreach (Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento in listaMonitoramento)
                {
                    Dominio.Entidades.Embarcador.Logistica.PosicaoAtual posicaoAtual = repositorioPosicaoAtual.BuscarPorPosicao(monitoramento?.UltimaPosicao?.Codigo ?? 0);

                    if (posicaoAtual != null)
                        retorno.Add(ConverterObjetoPosicionamentoVeiculo(posicaoAtual, carga));
                }
            }

            Dominio.ObjetosDeValor.WebService.Monitoramento.Rastreadores rastreadores = new Dominio.ObjetosDeValor.WebService.Monitoramento.Rastreadores()
            {
                PosicionamentoVeiculos = retorno
            };

            if (string.IsNullOrWhiteSpace(mensagem))
            {
                Servicos.Log.TratarErro($"Retorno : {(rastreadores != null ? Newtonsoft.Json.JsonConvert.SerializeObject(rastreadores) : string.Empty)}");
                return Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Monitoramento.Rastreadores>.CriarRetornoSucesso(rastreadores);
            }
            else
            {
                Servicos.Log.TratarErro($"Retorno: mensagem");
                return Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Monitoramento.Rastreadores>.CriarRetornoDadosInvalidos(mensagem, Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos);
            }

        }

        public Dominio.ObjetosDeValor.WebService.Retorno<bool> AdicionarNovaPosicao(Dominio.ObjetosDeValor.WebService.Rest.Monitoramento.Posicao posicao)
        {
            Retorno<bool> retorno = new Retorno<bool>();

            Repositorio.Embarcador.Logistica.Posicao repPosicao = new Repositorio.Embarcador.Logistica.Posicao(_unitOfWork);

            if (string.IsNullOrEmpty(posicao.placa) && string.IsNullOrEmpty(posicao.data_rastreadora))
            {
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                retorno.Mensagem = "Data Posição e Placa são campos obrigatórios";
                return retorno;
            }

            _unitOfWork.Start();

            List<Dominio.Entidades.Veiculo> veiculos = ObterVeiculoPorPlaca(posicao.placa);

            int totalVeiculos = veiculos?.Count ?? 0;
            for (int j = 0; j < totalVeiculos; j++)
            {
                if (Servicos.Embarcador.Logistica.WayPointUtil.ValidarCoordenadas(posicao.latitude, posicao.longitude))
                {
                    DateTime dataPosicao = DateTime.Parse(posicao.data);

                    DateTime dataVeiculo = DateTime.Parse(posicao.data_rastreadora);

                    if (dataPosicao != DateTime.MinValue && dataVeiculo != DateTime.MinValue)
                    {
                        Dominio.Entidades.Embarcador.Logistica.Posicao posicaoPendenteIntegracao = new Dominio.Entidades.Embarcador.Logistica.Posicao()
                        {
                            Data = dataPosicao,
                            DataVeiculo = dataVeiculo,
                            DataCadastro = DateTime.Now,
                            IDEquipamento = string.IsNullOrEmpty(posicao.id_dispositivo) ? veiculos[j].Codigo.ToString() : posicao.id_dispositivo,
                            Descricao = $"{posicao.latitude.ToString()}, {posicao.longitude.ToString()}",
                            Veiculo = veiculos[j],
                            Latitude = posicao.latitude,
                            Longitude = posicao.longitude,
                            Processar = ProcessarPosicao.Pendente,
                            Rastreador = EnumTecnologiaRastreador.Freto
                        };

                        repPosicao.Inserir(posicaoPendenteIntegracao);
                    }
                    else
                    {
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Mensagem = "Data Posição inválida";
                        return retorno;
                    }
                }
            }

            retorno.Objeto = true;
            retorno.Status = true;
            retorno.Mensagem = "Posição enviada para a fila!";

            _unitOfWork.CommitChanges();
            _unitOfWork.FlushAndClear();

            return retorno;

        }

        #endregion

        #region Métodos Privados


        private Dominio.ObjetosDeValor.WebService.Monitoramento.PosicionamentoVeiculo ConverterObjetoPosicionamentoConsultaMotorista(Dominio.Entidades.Usuario motoristaValidar)
        {
            Dominio.ObjetosDeValor.WebService.Monitoramento.PosicionamentoVeiculo posicionamento = new Dominio.ObjetosDeValor.WebService.Monitoramento.PosicionamentoVeiculo()
            {
                Transporte = string.Empty,
                IdDispositivo = motoristaValidar.CPF,
                UltimaPosicao = DateTime.MinValue,
                UltimaPosicaoString = "",
                Placa = string.Empty,
                Tipo = ""
            };

            return posicionamento;
        }

        private Dominio.ObjetosDeValor.WebService.Monitoramento.PosicionamentoVeiculo ConverterObjetoPosicionamentoVeiculo(Dominio.Entidades.Embarcador.Logistica.PosicaoAtual posicaoAtual, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Dominio.ObjetosDeValor.WebService.Monitoramento.PosicionamentoVeiculo posicionamento = new Dominio.ObjetosDeValor.WebService.Monitoramento.PosicionamentoVeiculo()
            {
                Transporte = carga?.CodigoCargaEmbarcador ?? string.Empty,
                IdDispositivo = (posicaoAtual?.Veiculo?.NumeroEquipamentoRastreador ?? string.Empty) != string.Empty ? posicaoAtual.Veiculo.NumeroEquipamentoRastreador : posicaoAtual?.IDEquipamento ?? string.Empty,
                UltimaPosicao = posicaoAtual.Data,
                UltimaPosicaoString = posicaoAtual.Data.ToString("yyyy-MM-ddTHH:mm:ss"),
                Placa = posicaoAtual?.Veiculo?.Placa ?? string.Empty,
                Tipo = ((posicaoAtual?.Veiculo?.TipoVeiculo ?? string.Empty) == string.Empty ? string.Empty : posicaoAtual.Veiculo.TipoVeiculo) == "0" ? "Tração" : "Reboque"
            };

            return posicionamento;
        }

        protected List<Dominio.Entidades.Veiculo> ObterVeiculoPorPlaca(string placa)
        {
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);

            List<Dominio.Entidades.Veiculo> veiculos = repVeiculo.BuscarListaPorPlaca(placa);

            return veiculos;
        }

        #endregion
    }
}