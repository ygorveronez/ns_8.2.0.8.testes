using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Repositorio.Embarcador.Integracao
{
    public class ArquivoEDICTe : RepositorioBase<Dominio.Entidades.Embarcador.Integracao.ArquivoEDICTe>
    {
        #region Construtores

        public ArquivoEDICTe(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private string ObterXmlCtes(List<int> listaCodigoCte)
        {
            XElement xmlCtes = new XElement("Ctes",
                from codigoCte in listaCodigoCte
                select new XElement("Id", codigoCte)
            );

            return xmlCtes.ToString();
        }

        #endregion

        #region Métodos Públicos

        public void VincularCteArquivoEDI(List<int> listaCodigoCte, int codigoLayoutEDI, string nomeArquivo)
        {
            if (listaCodigoCte?.Count > 0)
            {
                try
                {
                    var consultaVincularCteArquivoEDI = this.SessionNHiBernate.CreateSQLQuery("exec VincularCteArquivoEDI @XML_CTES = :xmlCtes, @CODIGO_LAYOUT_EDI = :codigoLayoutEDI, @NOME_ARQUIVO = :nomeArquivo");

                    consultaVincularCteArquivoEDI.SetParameter("xmlCtes", ObterXmlCtes(listaCodigoCte), NHibernate.NHibernateUtil.StringClob);
                    consultaVincularCteArquivoEDI.SetInt32("codigoLayoutEDI", codigoLayoutEDI);
                    consultaVincularCteArquivoEDI.SetString("nomeArquivo", nomeArquivo);

                    consultaVincularCteArquivoEDI.SetTimeout(300).ExecuteUpdate();
                }
                catch (System.Exception excecao)
                {
                    if (excecao.InnerException != null)
                        throw excecao.InnerException;

                    throw;
                }
            }
        }

        #endregion
    }
}
