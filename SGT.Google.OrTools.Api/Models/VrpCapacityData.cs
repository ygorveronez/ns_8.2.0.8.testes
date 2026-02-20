using System.Collections.Generic;
using System.Linq;

namespace Google.OrTools.Api.Models
{
    public class VrpCapacityData
    {
        public VrpCapacityData(VrpCapacity problem, bool vrptw = false)
        {
            this.Depot = problem.Locais.FindIndex(x => x.Deposito);

            //this.Demands = new long[problem.Locais.Count];
            this.Demands = new double[problem.Locais.Count];
            if (vrptw)
                this.TimeWindows = new int[problem.Locais.Count][];

            for (int i = 0; i < problem.Locais.Count; i++)
            {
                //this.Demands[i] = (long)problem.Locais[i].PesoTotal;
                this.Demands[i] = problem.Locais[i].PesoTotal;
                if (vrptw)
                {
                    if (problem.Locais[i].Janela == null)
                        problem.Locais[i].Janela = new Services.GoogleOrTools.TimeWindow(0, 1439, 0);
                    this.TimeWindows[i] = new int[2] { problem.Locais[i].Janela.start, problem.Locais[i].Janela.end };
                }
            }

            Services.DistanceMatrix distance = new Services.DistanceMatrix();
            this.DistanceMatrix = distance.ObterDistanceMatrix(problem);
            this.DurationMatrix = distance.DurationMatrix;

            //Vamos validar a capacidade total informada com 
            double totalDemanda = (from demanda in problem.Locais select demanda.PesoTotal).Sum();
            double capacidadeTransporte = (from veiculo in problem.Veiculos select veiculo.Quantidade * veiculo.Capacidade).Sum();

            List<long> capacidades = new List<long>();
            List<int> veiculos = new List<int>();
            List<int> veiculosModelo = new List<int>();
            List<long> paradas = new List<long>();
            List<long> paradasCanalEntrega = new List<long>();

            long stopsCanal = 999;
            if (problem.Locais.Any(x => x.PedidosConfig?.Count > 0))
                stopsCanal = (from obj in problem.Locais
                              where (obj.PedidosConfig?.Count ?? 0) > 0
                              select obj.PedidosConfig.Max(x => x.LimiteCanalEntrega))?.Max() ?? 0;
            if (stopsCanal == 0) stopsCanal = 999;

            foreach (Veiculo veiculo in problem.Veiculos)
            {
                for (int i = 0; i < veiculo.Quantidade; i++)
                {
                    long stops = (veiculo.QtdeMaximaEntregas > 0 ? veiculo.QtdeMaximaEntregas : problem.QtdeMaximaEntregas);
                    if (stops == 0) stops = 999;
                    capacidades.Add(veiculo.Capacidade);
                    veiculos.Add(veiculo.Codigo);
                    veiculosModelo.Add(veiculo.CodigoModelo);
                    paradas.Add(stops);
                    paradasCanalEntrega.Add(stopsCanal);
                }
            }

            //VRP se não tiver veiculos disponíveis suficiente.. não fechan nenhum carregamento, então vamos passar mais veiculos..
            //if (problem.GerarCarregamentosExtras || !vrptw)
            //{
            if (problem.GerarCarregamentosExtras)
            {
                Veiculo veiculoPadrao = problem.Veiculos.OrderByDescending(x => x.Capacidade).FirstOrDefault();

                if (vrptw)
                    veiculoPadrao = problem.Veiculos.OrderByDescending(x => x.Quantidade).FirstOrDefault();

                while (totalDemanda > capacidadeTransporte * 0.7)
                {
                    long stops = (veiculoPadrao.QtdeMaximaEntregas > 0 ? veiculoPadrao.QtdeMaximaEntregas : problem.QtdeMaximaEntregas);
                    if (stops == 0) stops = 999;
                    capacidades.Add(veiculoPadrao.Capacidade);
                    veiculos.Add(veiculoPadrao.Codigo);
                    veiculosModelo.Add(veiculoPadrao.CodigoModelo);
                    paradas.Add(stops);
                    capacidadeTransporte += veiculoPadrao.Capacidade;
                    paradasCanalEntrega.Add(stopsCanal);
                }
            }

            this.VehicleCapacities = capacidades.ToArray();
            this.Vehicle = veiculos.ToArray();
            this.VehicleModelo = veiculosModelo.ToArray();
            this.VehicleMaxStopeds = paradas.ToArray();
            this.VehicleMaxCanalEntrega = paradasCanalEntrega.ToArray();
        }

        //public long[] Demands { get; set; }
        public double[] Demands { get; set; }
        public int Depot { get; set; }
        public long[][] DistanceMatrix { get; set; }
        public long[][] DurationMatrix { get; set; }
        public int[][] TimeWindows { get; set; }
        public long[] VehicleCapacities { get; set; }
        public int[] Vehicle { get; }
        public long[] VehicleMaxStopeds { get; }
        public int[] VehicleModelo { get; }
        public long[] VehicleMaxCanalEntrega { get; }
    }
}