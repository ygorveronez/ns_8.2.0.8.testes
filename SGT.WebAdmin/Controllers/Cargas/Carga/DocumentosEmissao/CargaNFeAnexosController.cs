using Dominio.Entidades.Embarcador.Cargas;
using Repositorio;
using SGT.WebAdmin.Controllers.Anexo;

namespace SGT.WebAdmin.Controllers.Cargas.Carga.DocumentosEmissao
{
    [CustomAuthorize(new string[] { "RetornarEtapaTransportador", "ObterInformacoesGerais", "BuscarNFeCarga" }, "Cargas/Carga", "Logistica/AgendamentoColeta")]
    public class CargaNFeAnexosController : AnexoController<Dominio.Entidades.Embarcador.Cargas.CargaNFeAnexo, Dominio.Entidades.Embarcador.Cargas.Carga>
    {
        #region Construtores

        public CargaNFeAnexosController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Públicos

        protected override void PreecherInformacoesAdicionais(CargaNFeAnexo anexo, UnitOfWork unitOfWork)
        {
            anexo.OcultarParaTransportador = Request.GetBoolParam("OcultarParaTransportador");
        }

        #endregion Métodos Públicos
    }
}