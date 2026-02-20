namespace Dominio.Relatorios.Embarcador.DataSource.RH
{
    public class DiarioBordoSemanal
    {
        public int Numero { get; set; }
        public string Motorista { get; set; }
        public string Veiculo { get; set; }
        public string Carga { get; set; }
        public string DataInicio { get; set; }
        public string DataFim { get; set; }

        public string DataUmDiarioBordo { get; set; }
        public string DiaUmSemana { get; set; }

        public string DataDoisDiarioBordo { get; set; }
        public string DiaDoisSemana { get; set; }

        public string DataTresDiarioBordo { get; set; }
        public string DiaTresSemana { get; set; }

        public string DataQuatroDiarioBordo { get; set; }
        public string DiaQuatroSemana { get; set; }

        public string DataCincoDiarioBordo { get; set; }
        public string DiaCincoSemana { get; set; }

        public string DataSeisDiarioBordo { get; set; }
        public string DiaSeisSemana { get; set; }

        public string DataSeteDiarioBordo { get; set; }
        public string DiaSeteSemana { get; set; }
    }
}
