using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Mobile.Request
{
    [DataContract]
    public class Confirmar
    {
        [DataMember] public int clienteMultisoftware;
        [DataMember] public int codigoCargaEntrega;
        [DataMember] public List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Pedido> pedidos;
        [DataMember] public Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Coordenada coordenada;
        [DataMember] public Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Coordenada coordenadaDescarga;
        [DataMember] public string observacao;
        [DataMember] public int motivoRetificacao;
        [DataMember] public string justificativaEntregaForaRaio;
        [DataMember] public string inicioColetaEntrega;
        [DataMember] public string terminoColetaEntrega;
        [DataMember] public string dataConfirmacao;

        [DataMember] public int motivoFalhaGTA;
        [DataMember] public Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.DadosRecebedor dadosRecebedor;

        // Confirmação da chegada
        [DataMember] public string dataConfirmacaoChegada;
        [DataMember] public Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Coordenada coordenadaConfirmacaoChegada;

        [DataMember] public List<string> handlingUnitIds;
        [DataMember] public List<string> chavesNFe;
    }
}
