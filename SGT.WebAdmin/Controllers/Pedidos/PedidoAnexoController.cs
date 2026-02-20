using SGT.WebAdmin.Controllers.Anexo;
using System.Collections.Generic;

namespace SGT.WebAdmin.Controllers.Pedidos
{
    [CustomAuthorize("Pedidos/PedidoAnexo", "Pedidos/Pedido", "Cargas/Carga")]
    public class PedidoAnexoController : AnexoController<Dominio.Entidades.Embarcador.Pedidos.PedidoAnexo, Dominio.Entidades.Embarcador.Pedidos.Pedido>
    {
		#region Construtores

		public PedidoAnexoController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Métodos Protegidos Sobrescritos

		protected override bool IsPermitirAdicionarAnexo(Dominio.Entidades.Embarcador.Pedidos.Pedido entidade)
        {
            return true;
        }

        protected override bool IsPermitirExcluirAnexo(Dominio.Entidades.Embarcador.Pedidos.Pedido entidade)
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga");

            return permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_RemoverAnexosCarga);
        }

        #endregion
    }
}