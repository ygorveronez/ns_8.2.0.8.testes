using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.AngelLira.Pedidos
{

    #region Integracao Pedidos
    public class Pedidos
    {
        public List<Pedido> data { get; set; }
    }
    public class Pedido
    {
        public string ownId { get; set; }
        public int shipperId { get; set; }
        public string loadingDate { get; set; }
        public string originOwnId { get; set; }
      //  public double orderValue { get; set; }
       // public int routeId { get; set; }
       // public string temperatureRangeOwnId { get; set; }
        public double shippingValue { get; set; }
        //public int vehicleTypeId { get; set; }
        // public string notes { get; set; }
        //  public List<int> transportCompanies { get; set; }
        public List<Delivery> deliveries { get; set; }
    }


    public class documents
    {
        public List<Product> products { get; set; }

    }

    public class Product
    {
        public string productOwnId { get; set; }
        public int volumetry { get; set; }
        public int volumetryCodeId { get; set; }
        public int measure { get; set; }
        public int measureCodeId { get; set; }
    }

    public class Delivery
    {
        public string clientOwnId { get; set; }
        public string deliveryOwnId { get; set; }
        public DateTime deliveryDate { get; set; }
        public int sequence { get; set; }
        public List<documents> documents { get; set; }
    }

    #endregion


    #region Retorno Pedidos

    public class OwnIds
    {
        public List<string> ownIds { get; set; }
    }

    public class Retorno
    {
        public string ownId { get; set; }
        public int id { get; set; }
        public string notes { get; set; }
        public DateTime collectionDate { get; set; }
        public Resource resource { get; set; }

    }

    public class Truck
    {
        public int id { get; set; }
        public string plate { get; set; }
        public object prevStart { get; set; }
        public object prevEnd { get; set; }
        public int maintenance { get; set; }
        public bool availableTrip { get; set; }
        public bool insuranceProfileOk { get; set; }
        public bool availableAllocated { get; set; }
    }

    public class Trailer
    {
        public int sequence { get; set; }
        public int id { get; set; }
        public string plate { get; set; }
        public object prevStart { get; set; }
        public object prevEnd { get; set; }
        public int maintenance { get; set; }
        public bool availableTrip { get; set; }
        public bool insuranceProfileOk { get; set; }
        public bool availableAllocated { get; set; }
    }

    public class Driver
    {
        public int id { get; set; }
        public string name { get; set; }
        public bool availableTrip { get; set; }
        public bool availableAllocated { get; set; }
        public bool insuranceProfileOk { get; set; }
        public bool mainDriver { get; set; }
    }

    public class Client
    {
        public int id { get; set; }
        public int ownId { get; set; }
        public long cnpj { get; set; }
        public string name { get; set; }
    }

    public class Products
    {
        public int id { get; set; }
        public string name { get; set; }
        public string bond { get; set; }
        public int volume { get; set; }
        public string volumeMeasurement { get; set; }
    }

    public class Deliveries
    {
        public int id { get; set; }
        public DateTime date { get; set; }
        public string name { get; set; }
        public int bond { get; set; }
        public Products products { get; set; }
    }

    public class Resource
    {
        public Truck truck { get; set; }
        public List<Trailer> trailers { get; set; }
        public List<Driver> drivers { get; set; }
        public Client client { get; set; }
        public Deliveries deliveries { get; set; }
    }
    #endregion


}
