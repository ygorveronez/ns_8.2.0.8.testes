namespace Google.OrTools.Api.Models
{
    public class TspData
    {
        public TspData(Problem problema)
        {
            this.VehicleNumber = 1;
            this.Depot = 0;
            Services.DistanceMatrix distance = new Services.DistanceMatrix();
            this.DistanceMatrix = distance.ObterDistanceMatrix(problema);
            this.DurationMatrix = distance.DurationMatrix;
        }

        public int Depot { get; set; }
        public long[][] DistanceMatrix { get; set; }
        public long[][] DurationMatrix { get; set; }
        public int VehicleNumber { get; set; }
    }
}