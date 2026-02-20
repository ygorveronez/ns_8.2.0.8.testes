using System.Collections.Generic;

namespace Servicos.Embarcador.MDFe
{
    public class Averbacao
    {
        public static void IntegrarAverbacoesPendentesAutorizacao(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, string stringConexao)
        {
            Repositorio.AverbacaoMDFe repAverbacaoMDFe = new Repositorio.AverbacaoMDFe(unitOfWork);

            List<int> averbacoes = repAverbacaoMDFe.BuscarAverbacoes(Dominio.Enumeradores.StatusAverbacaoMDFe.AgEmissao, 100);

            Servicos.Embarcador.Carga.MDFe svcMDFe = new Servicos.Embarcador.Carga.MDFe(unitOfWork);

            int countAverbacoes = averbacoes.Count;

            for (var i = 0; i < countAverbacoes; i++)
                svcMDFe.EmitirAverbacao(averbacoes[i], unitOfWork);
        }

        public static void IntegrarAverbacoesPendentesCancelamento(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, string stringConexao)
        {
            Repositorio.AverbacaoMDFe repAverbacaoMDFe = new Repositorio.AverbacaoMDFe(unitOfWork);

            List<int> averbacoes = repAverbacaoMDFe.BuscarAverbacoes(Dominio.Enumeradores.StatusAverbacaoMDFe.AgCancelamento, 100);

            Servicos.Embarcador.Carga.MDFe svcMDFe = new Servicos.Embarcador.Carga.MDFe(unitOfWork);

            int countAverbacoes = averbacoes.Count;

            for (var i = 0; i < countAverbacoes; i++)
                svcMDFe.CancelarAverbacao(averbacoes[i], unitOfWork);
        }

        public static void IntegrarAverbacoesPendentesEncerramento(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, string stringConexao)
        {
            Repositorio.AverbacaoMDFe repAverbacaoMDFe = new Repositorio.AverbacaoMDFe(unitOfWork);

            List<int> averbacoes = repAverbacaoMDFe.BuscarAverbacoes(Dominio.Enumeradores.StatusAverbacaoMDFe.AgEncerramento, 100);

            Servicos.Embarcador.Carga.MDFe svcMDFe = new Servicos.Embarcador.Carga.MDFe(unitOfWork);

            int countAverbacoes = averbacoes.Count;

            for (var i = 0; i < countAverbacoes; i++)
                svcMDFe.EncerrarAverbacao(averbacoes[i], unitOfWork);
        }
    }
}
