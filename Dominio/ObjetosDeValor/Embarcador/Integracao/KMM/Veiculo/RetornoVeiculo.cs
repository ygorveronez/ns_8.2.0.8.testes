using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.KMM
{
    public class RetornoVeiculo
    {
        public bool Success { get; set; }
        public int Code { get; set; }
        public ResultVeiculo Result { get; set; }


    }
    public class VeiculoRet
    {
        public int Veiculo_Id { get; set; }
        public string Frota { get; set; }
        public string Placa { get; set; }
    }
    public class MarcaRet
    {
        public int Marca_Id { get; set; }
    }
    public class ModeloRet
    {
        public int Modelo_Id { get; set; }
    }
    public class LocalidadeRet
    {
        public int Municipio_Id { get; set; }
    }
    public class CarroceriaRet
    {
        public int Tipo_Carroceria_Id { get; set; }
        public int Num_Tipo_Carroceria { get; set; }
        public string Descricao_Carroceria { get; set; }
        public int Agrupamento_Id { get; set; }
        public int Num_Agrupamento { get; set; }
        public string Descricao_Agrupamento { get; set; }
    }
    public class ResultVeiculo
    {
        public List<VeiculoRet> Veiculo { get; set; }
        public List<MarcaRet> Marca { get; set; }
        public List<ModeloRet> Modelo { get; set; }
        public List<LocalidadeRet> Municipios { get; set; }
        public List<CarroceriaRet> Carroceria { get; set; }
        public double Tempo_Execucao { get; set; }
        public string Lines { get; set; }
    }
}
