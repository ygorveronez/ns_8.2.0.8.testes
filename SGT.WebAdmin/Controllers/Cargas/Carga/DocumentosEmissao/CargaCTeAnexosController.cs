using SGT.WebAdmin.Controllers.Anexo;

namespace SGT.WebAdmin.Controllers.Cargas.Carga.DocumentosEmissao
{
    [CustomAuthorize(new string[] { "RetornarEtapaTransportador", "ObterInformacoesGerais", "BuscarNFeCarga" }, "Cargas/Carga", "Logistica/AgendamentoColeta")]
    public class CargaCTeAnexosController : AnexoController<Dominio.Entidades.Embarcador.Cargas.CargaNFeAnexo, Dominio.Entidades.Embarcador.Cargas.Carga>
    {
        #region Construtores

        public CargaCTeAnexosController(Conexao conexao) : base(conexao) { }

        #endregion       
    }
}