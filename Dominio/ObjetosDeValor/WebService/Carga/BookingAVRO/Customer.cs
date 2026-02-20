namespace Dominio.ObjetosDeValor.WebService.Carga.BookingAVRO
{
    public class Customer
    {
        public CustomerId customerId { get; set; }
        public string personType { get; set; }
        public string customerName { get; set; }
        public string taxID { get; set; }
        public string stateRegistration { get; set; }
        public double? cpf { get; set; }
        public int facilityId { get; set; }
        public string zipCode { get; set; }
        public string address { get; set; }
        public string number { get; set; }
        public string district { get; set; }
        public City city { get; set; }
        public State state { get; set; }
    }
}
