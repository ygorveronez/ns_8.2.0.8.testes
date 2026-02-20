using System;
namespace Servicos
{
    public class StatusConsultaImpressaoUnidade
    {
        public static bool VerificarStatusConsultaImpressaoUnidade(int numeroUnidade, Dominio.Enumeradores.TipoObjetoConsulta documento, bool consultando, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.StatusConsultaImpressaoUnidade repStatusConsultaImpressaoUnidade = new Repositorio.StatusConsultaImpressaoUnidade(unitOfWork);
            Dominio.Entidades.StatusConsultaImpressaoUnidade statusConsultaImpressaoUnidade = repStatusConsultaImpressaoUnidade.BuscarPorUnidadeDocumento(numeroUnidade, documento);
            if (statusConsultaImpressaoUnidade == null)
            {
                statusConsultaImpressaoUnidade = new Dominio.Entidades.StatusConsultaImpressaoUnidade();
                statusConsultaImpressaoUnidade.Consultando = consultando;
                statusConsultaImpressaoUnidade.Data = DateTime.Now;
                statusConsultaImpressaoUnidade.NumeroDaUnidade = numeroUnidade;
                statusConsultaImpressaoUnidade.Documento = documento;
                repStatusConsultaImpressaoUnidade.Inserir(statusConsultaImpressaoUnidade);

                return true;
            }
            else if (statusConsultaImpressaoUnidade.Consultando && consultando)
            {
                TimeSpan tempo = (DateTime.Now - statusConsultaImpressaoUnidade.Data);

                if (tempo.Minutes > 5)
                    return true;
                else
                    return false;
            }
            else
            {
                statusConsultaImpressaoUnidade.Consultando = consultando;
                statusConsultaImpressaoUnidade.Data = DateTime.Now;
                repStatusConsultaImpressaoUnidade.Atualizar(statusConsultaImpressaoUnidade);

                return true;
            }
        }


    }
}
