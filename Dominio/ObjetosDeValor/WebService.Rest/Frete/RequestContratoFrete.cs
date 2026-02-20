namespace Dominio.ObjetosDeValor.WebService.Rest.Frete
{
    public class RequestContratoFrete : RequestPaginacao
    {
        public string DataInicial { get; set; }
        public string DataFinal { get; set; }

    }
}
