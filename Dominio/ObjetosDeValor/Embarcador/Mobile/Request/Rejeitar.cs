using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Mobile.Request
{
    [DataContract]
    public class Rejeitar
    {
        [DataMember] public int clienteMultisoftware;
        [DataMember] public int codigoCargaEntrega;
        [DataMember] public int codigoMotivo;
        [DataMember] public List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Pedido> pedidos;
        [DataMember] public Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Coordenada coordenada;
        [DataMember] public Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Coordenada coordenadaDescarga;
        [DataMember] public string observacao;
        [DataMember] public int motivoRetificacao;
        [DataMember] public bool devolucaoParcial;
        [DataMember] public List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Produto> produtos;

        // Carga/Descarga
        [DataMember] public string inicioCarregamento;
        [DataMember] public string terminoCarregamento;
        [DataMember] public string inicioDescarga;
        [DataMember] public string terminoDescarga;

        [DataMember] public int motivoFalhaGTA;
        [DataMember] public Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.DadosRecebedor dadosRecebedor;

        // Confirmação da chegada
        [DataMember] public string dataConfirmacaoChegada;
        [DataMember] public Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Coordenada coordenadaConfirmacaoChegada;
    }
}
