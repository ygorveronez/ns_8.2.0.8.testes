using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;

namespace Dominio.Relatorios.Embarcador.DataSource.Cargas.ControleEntrega
{
    public class CargaEntregaColeta
    {
        #region Propriedades
        public int Codigo { get; set; }

        public SituacaoEntrega SituacaoEntrega { get; set; }

        public string CodigoCargaEmbarcador { get; set; }

        public string DadosPedidos { set { if (value != null) Pedidos = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Pedidos>>(value); } }
        public List<Pedidos> Pedidos { get; set; }

        public int NumeroReboques { get; set; }

        public bool ExigePlacaTracao { get; set; }

        #endregion Propriedades

        #region Propriedades Com Regras

        public string Descricao
        {
            get { return Codigo.ToString(); }
        }

        public string Situacao
        {
            get { return SituacaoEntregaHelper.ObterDescricao(SituacaoEntrega); }
        }

        public string NumeroPedidos
        {
            get
            {
                return Pedidos != null ? string.Join(", ", Pedidos.Select(p => p.NumeroPedidoEmbarcador)) : string.Empty;
            }
        }

        public string Remetente
        {
            get
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(Pedidos?.Select(p => new { Codigo = p.CodigoRemetente, Descricao = p.DescricaoRemetente }).FirstOrDefault());
            }
        }

        public string Remetentes
        {
            get
            {
                return Pedidos != null ? string.Join(", ", Pedidos.Select(p => p.DescricaoRemetente)) : string.Empty;
            }
        }

        public string Destinatario
        {
            get
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(Pedidos?.Select(p => new { Codigo = p.CodigoDestinatario, Descricao = p.DescricaoDestinatario }).FirstOrDefault());
            }
        }

        public string Destinatarios
        {
            get
            {
                return Pedidos != null ? string.Join(", ", Pedidos.Select(p => p.DescricaoDestinatario)) : string.Empty;
            }
        }

        #endregion Propriedades Com Regras
    }

    public class Pedidos
    {
        public int Codigo { get; set; }

        public string Descricao
        {
            get { return NumeroPedidoEmbarcador; }
        }

        public string NumeroPedidoEmbarcador { get; set; }

        public long CodigoRemetente { get; set; }
        public string DescricaoRemetente { get; set; }

        public long CodigoDestinatario { get; set; }
        public string DescricaoDestinatario { get; set; }
    }
}