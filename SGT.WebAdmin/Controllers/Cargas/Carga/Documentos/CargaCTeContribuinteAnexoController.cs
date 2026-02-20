using Dominio.Entidades.Embarcador.Cargas;
using SGT.WebAdmin.Controllers.Anexo;

namespace SGT.WebAdmin.Controllers.Cargas.Carga.Frete
{
    [CustomAuthorize("Cargas/ControleEntrega", "Logistica/Monitoramento", "Cargas/Carga", "Logistica/JanelaCarregamento", "GestaoPatio/FluxoPatio", "Ocorrencias/AutorizacaoOcorrencia")]
    public class ContribuinteCargaCTeAnexoController : AnexoController<Dominio.Entidades.Embarcador.Cargas.ContribuinteCargaCTeAnexo, Dominio.Entidades.Empresa>
    {
		#region Construtores

		public ContribuinteCargaCTeAnexoController(Conexao conexao) : base(conexao) { }

		#endregion

		protected override void PreecherInformacoesAdicionais(ContribuinteCargaCTeAnexo anexo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            int codigoCargaCTe = Request.GetIntParam("CargaCTe");

            Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCTe.BuscarPorCodigo(codigoCargaCTe);

            if (cargaCTe != null)
            {
                anexo.CargaCTe = cargaCTe;

                cargaCTe.SituacaoDocumentoContribuinte = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoContribuinte.AgAnaliseDocumentacao;
                repCargaCTe.Atualizar(cargaCTe);

            }
        }
    }
}