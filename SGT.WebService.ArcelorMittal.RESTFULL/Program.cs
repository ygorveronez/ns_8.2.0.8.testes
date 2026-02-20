using Dominio.Ferroviario;
using Servicos.Http;
using System.Dynamic;
using System.Text;
using System.Xml.Linq;

var builder = WebApplication.CreateBuilder(args);
HttpClientRegistration.RegisterClients();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();



// Minimal APIs 
app.MapPost("/", async context =>
{
    try
    {
        //Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        //Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Base.Conexao.StringConexao);

        using (StreamReader reader = new StreamReader(context.Request.Body, Encoding.UTF8))
        {
            string xmlSoap = await reader.ReadToEndAsync();
            string xmlSoapRetorno;
            dynamic dynamicObject = ConvertSoapToDynamic(xmlSoap);
            //var teste = dynamicObject.Envelope.Body.EventoFerroviario.eventos.Evento.tipoEvento;
            Servicos.Log.TratarErro($"EventoFerroviario", "EventoFerroviario");
            Servicos.Log.TratarErro($"protocoloEnvio : {dynamicObject.Envelope.Body.EventoFerroviario.protocoloEnvio} Codigo: {dynamicObject.Envelope.Body.EventoFerroviario.CNPJEmissor}", "EventoFerroviario");
            foreach (var eventoKeyValue in dynamicObject.Envelope.Body.EventoFerroviario.eventos)
            {

                Servicos.Log.TratarErro($"Eventos", "EventoFerroviario");
                if (eventoKeyValue.Value != null)
                {
                    var evento = (dynamic)eventoKeyValue.Value;
                    Servicos.Log.TratarErro($"Tipo Evento : {evento.tipoEvento} Codigo: {evento.codigoEvento}", "EventoFerroviario");
                }
            }

            Dominio.Ferroviario.MRetornoEnvio retorno = new MRetornoEnvio();
            retorno.data.statusEnvio = TStatus.OK;





            var serializer = new System.Xml.Serialization.XmlSerializer(retorno.GetType());
            using (var stringWriter = new StringWriter())
            {
                serializer.Serialize(stringWriter, retorno);
                xmlSoapRetorno = stringWriter.ToString();
            }
            
            await context.Response.WriteAsync(xmlSoapRetorno);
        }
    }
    catch (Exception e)
    {

        await context.Response.WriteAsync(e.Message);
    }

}).WithName("Monitoramento");





app.MapPost("/hellow/", () =>
{
    return "hellow";

}).WithName("GetCustomerById");

app.MapGet("/Parametro/{id}", (int id) =>
{
    return true;

}).WithName("GetCustomeById");


app.UseHttpsRedirection();
app.Run();



static dynamic ConvertSoapToDynamic(string xml)
{
    dynamic dynamicObject = new ExpandoObject();

    using (var reader = new StringReader(xml))
    {
        var document = XDocument.Load(reader);

        dynamicObject.Envelope = new ExpandoObject();
        dynamicObject.Envelope.Body = new ExpandoObject();

        foreach (var element in document.Descendants())
        {
            AddProperties(dynamicObject.Envelope.Body, element);
        }
    }

    return dynamicObject;
}

static void AddProperties(ExpandoObject expando, XElement element)
{
    var dictionary = (IDictionary<string, object>)expando;

    if (element.HasElements)
    {
        var childExpando = new ExpandoObject();
        foreach (var childElement in element.Elements())
        {
            AddProperties(childExpando, childElement);
        }

        dictionary[element.Name.LocalName] = childExpando;
    }
    else
    {
        dictionary[element.Name.LocalName] = element.Value;
    }
}