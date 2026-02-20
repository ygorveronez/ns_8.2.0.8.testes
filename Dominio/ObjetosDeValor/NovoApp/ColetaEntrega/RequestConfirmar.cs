using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.NovoApp.ColetaEntrega
{
    public class RequestConfirmar
    {
        public int clienteMultisoftware { get; set; }

        public int codigoCargaEntrega { get; set; }
        
        /// <summary>
        /// Aqui fica os dados dos pedidos da coleta/entrega. Por exemplo, os dados de cada produto que está sendo transportado.
        /// </summary>
        public List<Dominio.ObjetosDeValor.NovoApp.ColetaEntrega.PedidoConfirmacao> pedidos { get; set; }

        public Dominio.ObjetosDeValor.NovoApp.ColetaEntrega.DadosRIC dadosRic { get; set; }

        /// <summary>
        /// Coordenada de quando o app confirma a coleta/entrega. 
        /// </summary>
        public Dominio.ObjetosDeValor.NovoApp.Comum.Coordenada coordenadaConfirmacao { get; set; }

        /// <summary>
        /// Observação arbitrária do motorista sobre a coleta/entrega
        /// </summary>
        public string observacao { get; set; }

        /// <summary>
        /// Quando o motorista quer editar uma coleta/entrega, ele retifica essa entrega.
        /// Esse campo recebe o id do motivo dessa retificação. 
        /// </summary>
        public int motivoRetificacao { get; set; }

        /// <summary>
        /// Quando o motorista não consegue ler as chaves de nota fiscal ("chavesNFe" abaixo) que deveria, o motivo
        /// dessa falha fica armazenado aqui
        /// </summary>
        public int motivoFalhaNotaFiscal { get; set; }

        /// <summary>
        /// Se o motorista tiver que fazer a entrega fora do raio definido do cliente,
        /// ele terá que preencher esse campo para justificar essa ação.
        /// </summary>
        public string justificativaEntregaForaRaio { get; set; }

        /// <summary>
        /// Data de início do carregamento/descarregamento.
        /// Se o app não tiver uma forma do motorista dizer explicitamente que o evento foi iniciado,
        /// então a data será a mesma que a dataConfirmacao.
        /// </summary>
        public long dataInicio { get; set; }

        /// <summary>
        /// Data de finalização do carregamento/descarregamento.
        /// Se o app não tiver uma forma do motorista dizer explicitamente que o evento foi finalizado,
        /// então a data será a mesma que a dataConfirmacao.
        /// </summary>
        public long dataTermino { get; set; }

        /// <summary>
        /// Data da finalização de todo o processo da coleta/entrega no app.
        /// </summary>
        public long dataConfirmacao { get; set; }

        /// <summary>
        /// Quando transportando animais, um documento chamado GTA é necessário.
        /// O app lê o código de barras dele. Caso a leitura não tenha sido feita, esse campo
        /// guarda o id do motivo pelo qual o documento não pôde ser lido.
        /// </summary>
        public int motivoFalhaGTA { get; set; }

        /// <summary>
        /// Em alguns casos é necessário requisitar os dados do recebedor no app.
        /// Quando for necessário, eles ficarão aqui.
        /// </summary>
        public Dominio.ObjetosDeValor.NovoApp.ColetaEntrega.DadosRecebedor dadosRecebedor { get; set; }

        /// <summary>
        /// Data de quando o motorista aperta em "Confirmar chegada"
        /// </summary>
        public long dataConfirmacaoChegada { get; set; }

        /// <summary>
        /// Em alguns casos é necessário apertar um botão indicando que foi o motorista chegou no
        /// local da coleta/entrega. Quando for necessário, a coordenada ficará aqui.
        /// </summary>
        public Dominio.ObjetosDeValor.NovoApp.Comum.Coordenada coordenadaConfirmacaoChegada { get; set; }

        /// <summary>
        /// Quando há integração com o Mercado Livre, o motorista tem que ler vários códigos de barras que estão adesivados
        /// em pallets/caixas que são chamados de Handling Unit Ids. Esses códigos são armazenados para serem integrados posteriormente.
        /// </summary>
        public List<string> handlingUnitIds { get; set; }

        /// <summary>
        /// Em alguns casos, é necessário que o motorista leia com o app os códigos de barras
        /// de NFes. Os códigos lidos ficam aqui.
        /// </summary>
        public List<string> chavesNFe { get; set; }

        /// <summary>
        /// Quando é necessário que o produtor/recebedor avalie a coleta/entrega, a nota fica nessa
        /// propriedade
        /// </summary>
        public int avaliacaoColetaEntrega { get; set; }

        /// <summary>
        /// Campo recebido de integração com a Trizy. Se configurado, busca a CargaEntrega com base nesse campo
        /// </summary>
        public string IdTrizy { get; set; }
    }
}
