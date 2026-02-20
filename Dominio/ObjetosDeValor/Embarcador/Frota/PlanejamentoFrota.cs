using System;
using System.Collections.Generic;
using System.Linq;

namespace Dominio.ObjetosDeValor.Embarcador.Frota
{
    public class PlanejamentoFrota
    {
        #region propriedades privadas

        private string _formatDataHora = "dd-MM-yyyy HH:mm:ss";

        #endregion

        #region Propriedades

        public int CodigoFrota { get; set; }
        public int VeiculoTracao { get; set; }
        public int VeiculoReboque1 { get; set; }
        public int VeiculoReboque2 { get; set; }
        public string ModeloVeicularVeiculoTracao { get; set; }
        public string ModeloVeicularReboque1 { get; set; }
        public string ModeloVeicularReboque2 { get; set; }

        public string ModeloVeicularFormatado { get { return !string.IsNullOrEmpty(ModeloVeicularVeiculoTracao) ? ModeloVeicularVeiculoTracao : !string.IsNullOrEmpty(ModeloVeicularReboque1) ? ModeloVeicularReboque1 : !string.IsNullOrEmpty(ModeloVeicularReboque2) ? ModeloVeicularReboque2 : ""; } }

        public string PlacaVeiculoTracao { get; set; }
        public string PlacaVeiculoReboque1 { get; set; }
        public string PlacaVeiculoReboque2 { get; set; }

        public int MotoristaPrincipal { get; set; }
        public int MotoristaAuxiliar { get; set; }
        public string NomeMotoristaPrincipal { get; set; }
        public string NomeMotoristaAuxiliar { get; set; }

        public int CargaVinculada { get; set; }
        public string CodigoCargaEmbarcadorVinculada { get; set; }
        public string LocalidadeOrigem { get; set; }
        public string LocalidadeDestino { get; set; }

        public int LocalVeiculoFrota { get; set; }
        public string DescricaoLocalVeiculoFrota { get; set; }
        public string UFDescricaoLocalVeiculoFrota { get; set; }
        public int PaisLocalVeiculoFrota { get; set; }

        public string DescricaoLocalVeiculoFrotaReboque1 { get; set; }
        public string UFDescricaoLocalVeiculoFrotaReboque1 { get; set; }
        public int PaisLocalVeiculoFrotaReboque1 { get; set; }

        public string DescricaoLocalVeiculoFrotaReboque2 { get; set; }
        public string UFDescricaoLocalVeiculoFrotaReboque2 { get; set; }
        public int PaisLocalVeiculoFrotaReboque2 { get; set; }

        public bool Ativo { get; set; }

        public bool PossuiProgramacaoFutura { get; set; }
        public string DescricaoStatus { get; set; }

        public string LocalVeiculoFormatado { get { return !string.IsNullOrEmpty(DescricaoLocalVeiculoFrota) ? DescricaoLocalVeiculoFrota + " - " + UFDescricaoLocalVeiculoFrota : !string.IsNullOrEmpty(DescricaoLocalVeiculoFrotaReboque1) ? DescricaoLocalVeiculoFrotaReboque1 + " - " + UFDescricaoLocalVeiculoFrotaReboque1 : !string.IsNullOrEmpty(DescricaoLocalVeiculoFrotaReboque2) ? DescricaoLocalVeiculoFrotaReboque2 + " - " + UFDescricaoLocalVeiculoFrotaReboque2 : "----"; } }

        public string LocalPlanejamentoFormatado { get; set; }
        public int PaisDestino { get; set; }

        public DateTime DataInicioVigencia { get; set; }
        public virtual string DataInicioVigenciaFormatada { get { return DataInicioVigencia.ToString(_formatDataHora); } }
        public DateTime? DataFimVigencia { get; set; }
        public virtual string DataFimVigenciaFormatada { get { return DataFimVigencia?.ToString(_formatDataHora) ?? ""; } }

        public double LatitudeFrota { get; set; }
        public double LongitudeFrota { get; set; }

        public DateTime? DataManutencaoTracao { get; set; }
        public DateTime? DataManutencaoReboque1 { get; set; }
        public DateTime? DataManutencaoReboque2 { get; set; }

        public bool ExisteManutencaoProxima { get; set; }
        public bool ManutencaoExpirada { get; set; }

        public string DadosPlanejamento { set { if (value != null) Planejamento = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Planejamento>>(value); } }
        public List<Planejamento> Planejamento { get; set; }

        public double LatitudeCidade { get; set; }
        public double LongitudeCidade { get; set; }
        public double Distancia { get; set; }
        public string DistanciaKM { get { return Distancia > 0 ? (Distancia / 1000).ToString("N2") + " Km" : ""; } }

        public DateTime? DataPosicao { get; set; }
        public virtual string DataUltimaPosicao
        {
            get
            {
                if (DataPosicao.HasValue)
                {
                    return DataPosicao.Value.ToString(_formatDataHora);
                }
                else
                    return "";
            }
        }

        public virtual string PosicaoAtual { get; set; }

        public string DataDisponivelCarregamento { get; set; }
        public string TempoPercorrerDistancia(DateTime dataPesquisa)
        {
            if (Distancia > 0)
            {
                var distanciaKM = Distancia / 1000;
                var tempo = Convert.ToInt32((distanciaKM * 1.3) / 45 * 60); //esta usando velocidade media de 45km por hora....
                if (tempo <= 0)
                    return dataPesquisa.ToString("dd/MM/yyyy HH:mm");
                else
                    return dataPesquisa.AddMinutes(tempo).ToString("dd/MM/yyyy HH:mm");
            }
            else
                return "";
        }

        #endregion

        #region Metodos

        public bool ManutencaoProxima(DateTime DataInformada)
        {
            if (DataManutencaoTracao.HasValue && (DataManutencaoTracao.Value.Date > DataInformada.Date) && (DataManutencaoTracao.Value.Date - DataInformada.Date).Days <= 8)
                return true;

            if (DataManutencaoReboque1.HasValue && (DataManutencaoReboque1.Value.Date > DataInformada.Date) && (DataManutencaoReboque1.Value.Date - DataInformada.Date).Days <= 8)
                return true;

            if (DataManutencaoReboque2.HasValue && (DataManutencaoReboque2.Value.Date > DataInformada.Date) && (DataManutencaoReboque2.Value.Date - DataInformada.Date).Days <= 8)
                return true;


            return false;
        }

        public bool ExpirouManutencao(DateTime DataInformada)
        {

            if (DataManutencaoTracao.HasValue && (DataManutencaoTracao.Value.Date < DataInformada.Date))
                return true;

            if (DataManutencaoReboque1.HasValue && (DataManutencaoReboque1.Value.Date < DataInformada.Date))
                return true;

            if (DataManutencaoReboque2.HasValue && (DataManutencaoReboque2.Value.Date < DataInformada.Date))
                return true;


            return false;
        }

        public string BuscarLocalVeiculo(List<Planejamento> planejamentosFrota, DateTime DataInformada)
        {
            if (planejamentosFrota == null)
                return LocalVeiculoFormatado;

            if (planejamentosFrota.Any(x => x.DataCarregamento.Date <= DataInformada.Date)) //possui planejamento anterior, vamos obter o destino;
            {
                var planejamentoAnteriordata = planejamentosFrota.Where(x => x.DataCarregamento.Date <= DataInformada.Date).FirstOrDefault();

                if (planejamentoAnteriordata != null)
                {
                    if (!string.IsNullOrEmpty(planejamentoAnteriordata.cidadeFim))
                        return planejamentoAnteriordata.cidadeFim + " - " + planejamentoAnteriordata.estadoFim;
                    else
                        return planejamentoAnteriordata.Destinos;
                }
                else
                    return LocalVeiculoFormatado;
            }
            else
                return LocalVeiculoFormatado;
        }

        public int BuscarPaisDestinoVeiculo(List<Planejamento> planejamentosFrota, DateTime DataInformada)
        {
            //1058 = brasil;

            if (planejamentosFrota == null)
                return PaisLocalVeiculoFrota == 0 ? 1058 : PaisLocalVeiculoFrota;

            if (planejamentosFrota.Any(x => x.DataCarregamento.Date < DataInformada.Date)) //possui planejamento anterior, deve obter o destino;
            {
                var planejamentoAnteriordata = planejamentosFrota.Where(x => x.DataCarregamento.Date < DataInformada.Date).FirstOrDefault();

                if (planejamentoAnteriordata != null)
                {
                    if (planejamentoAnteriordata.paisFim > 0 && planejamentoAnteriordata.paisFim != 1058)
                        return planejamentoAnteriordata.paisFim;
                    else
                        return 1058;
                }
                else
                    return 1058;
            }
            else
                return 1058;
        }

        public bool VerificarProgramacaoFutura(List<Planejamento> planejamentosFrota, DateTime DataInformada)
        {
            if (planejamentosFrota == null)
                return false;

            if (planejamentosFrota.Any(x => x.DataCarregamento > DataInformada))
                return true;

            return false;
        }

        public string ObterStatusFrota(List<Planejamento> planejamentosFrota, DateTime DataInformada)
        {

            if (DataManutencaoTracao.HasValue && (DataManutencaoTracao.Value.Date == DataInformada.Date))
                return "Em Manutenção";

            if (DataManutencaoReboque1.HasValue && (DataManutencaoReboque1.Value.Date == DataInformada.Date))
                return "Em Manutenção";

            if (DataManutencaoReboque2.HasValue && (DataManutencaoReboque2.Value.Date == DataInformada.Date))
                return "Em Manutenção";

            if (planejamentosFrota == null)
                return "Disponível";

            if (planejamentosFrota.Any(x => x.DataFimViagemPrevista.HasValue && x.DataFimViagemPrevista.Value.Date == DataInformada.Date))
                return "Em Descarregamento";

            if (planejamentosFrota.Any(x => x.DataCarregamento.Date == DataInformada.Date))
                return "Em Carregamento";

            if (planejamentosFrota.Any(x => (x.DataInicioViagem.HasValue && x.DataInicioViagem.Value.Date <= DataInformada.Date) || (x.DataInicioViagemPrevista.HasValue && x.DataInicioViagemPrevista.Value.Date <= DataInformada.Date)))
            { //teve incio viagem na data pesquisada
                var retorno = "Em Viagem";
                Planejamento planjementoEmViagem = planejamentosFrota.Where(x => (x.DataInicioViagem.HasValue && x.DataInicioViagem.Value.Date <= DataInformada.Date) || (x.DataInicioViagemPrevista.HasValue && x.DataInicioViagemPrevista.Value.Date <= DataInformada.Date)).FirstOrDefault();

                if (planjementoEmViagem != null && ((planjementoEmViagem.DataFimViagemPrevista.HasValue && planjementoEmViagem.DataFimViagemPrevista.Value.Date <= DataInformada.Date) || (planjementoEmViagem.DataFimViagem.HasValue && planjementoEmViagem.DataFimViagem.Value.Date <= DataInformada.Date))) // ja encerrou viagem, deve ficar disponivel
                    retorno = "Disponível";

                return retorno;
            }

            if (planejamentosFrota.Any(x => (x.DataInicioViagem.HasValue && x.DataInicioViagem.Value.Date == DataInformada.Date) || (x.DataInicioViagemPrevista.HasValue && x.DataInicioViagemPrevista.Value.Date == DataInformada.Date)))
                return "Em Viagem";

            if (!planejamentosFrota.Any(x => x.DataCarregamento.Date == DataInformada.Date))
                return "Disponível";


            return "";
        }

        #endregion
    }
}
