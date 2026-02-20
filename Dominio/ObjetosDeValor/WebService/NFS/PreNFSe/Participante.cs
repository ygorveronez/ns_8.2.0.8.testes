namespace Dominio.ObjetosDeValor.WebService.NFS.PreNFSe
{
    public class Participante
    {
        public string CpfCnpj { get; set; }

        public string Nome { get; set; }

        public string inscricaoEstadual { get; set; }

        public Endereco Endereco { get; set; }
    }
}
