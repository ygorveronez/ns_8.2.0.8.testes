using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.NovoApp.ColetaEntrega
{
    public class RequestLancarDespesaViagem
    {
        public int clienteMultisoftware { get; set; }

        public int codigoCargaEntrega { get; set; }
        public int codigoCarga { get; set; }

        /// <summary>
        /// Data em que o usuário mobile registrou a criação da ocorrência no APP.
        /// </summary>
        public long data { get; set; }

        /// <summary>
        /// Código do motivo da rejeição da coleta/entrega
        /// </summary>
        public int codigoMotivoRejeicao { get; set; }

        /// <summary>
        /// Quando a rejeição for uma devolução, guarda se ela foi parcial ou não
        /// </summary>
        public bool devolucaoParcial { get; set; }

        /// <summary>
        /// Aqui fica os dados das notas e produtos que participam da rejeição.
        /// </summary>
        public List<NotaRejeicao> notasFiscais { get; set; }

        /// <summary>
        /// Coordenada de quando o app confirma a coleta/entrega. 
        /// </summary>
        public Dominio.ObjetosDeValor.NovoApp.Comum.Coordenada coordenadaRejeicao { get; set; }

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
        /// Data de início do carregamento/descarregamento.
        /// Se o app não tiver uma forma do motorista dizer explicitamente que o evento foi iniciado,
        /// então a data será a mesma que a dataConfirmacao.
        /// É a mesma que a da RequestConfirmar.
        /// </summary>
        public long dataInicio { get; set; }

        /// <summary>
        /// Em alguns casos é necessário apertar um botão indicando que foi o motorista chegou no
        /// local da coleta/entrega. Quando for necessário, a coordenada ficará aqui.
        /// </summary>
        public Dominio.ObjetosDeValor.NovoApp.Comum.Coordenada coordenadaConfirmacaoChegada { get; set; }

        /// <summary>
        /// Se a ocorrência precisar de imagens, elas serão mandadas aqui em base64
        /// </summary>
        public List<string> imagens { get; set; }


        /// <summary>
        /// Indica a quantidade de imagens que serão enviadas pelo APP
        /// </summary>
        public int quantidadeImagens { get; set; }

        public int codigoJustificativaDespesaViagem { get; set; }
        public decimal ValorDespesaViagem { get; set; }
    }
}
