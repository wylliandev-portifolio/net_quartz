using Quartz;
using System.IO.Compression;

namespace ServicoQuartz.Jobs
{
    public class ExportarImagensJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine("Iniciando compactação do dados");

            SetarVariavelAmbiente();

            CompactarPastaCategorias();
            return Task.CompletedTask;
        }

        private void SetarVariavelAmbiente()
        {
            string variavel = "PATH_API_GESTOR";
            string valor = "D:\\Projetos\\gestor20\\src\\Gestor.Web\\bin\\Debug\\net7.0-windows";

            // Define a variável de ambiente para o processo atual
            Environment.SetEnvironmentVariable(variavel, valor);

            // Define a variável de ambiente para o usuário
            Environment.SetEnvironmentVariable(variavel, valor, EnvironmentVariableTarget.User);

            // Define a variável de ambiente para a máquina
            Environment.SetEnvironmentVariable(variavel, valor, EnvironmentVariableTarget.Machine);
        }

        private string? BuscarVariavelAmbiente()
        {
            string variavel = "PATH_API_GESTOR";
            string? valor = Environment.GetEnvironmentVariable(variavel);

            if (!string.IsNullOrEmpty(valor))
                Console.WriteLine($"O valor da variável de ambiente {variavel} é: {valor}");
            else
                Console.WriteLine($"A variável de ambiente {variavel} não está definida.");

            return valor;

        }

        private void CompactarPasta(string categoria, string pastaPath)
        {
            if (!Directory.Exists(pastaPath))
                throw new Exception("A pasta especificada não existe.");

            try
            {
                string folderPathImagem= Path.Combine(pastaPath, "Imagens");

                if (!Directory.Exists(folderPathImagem))
                    throw new Exception("A pasta especificada não existe.");

                string sourceDirectory = Path.Combine(folderPathImagem, categoria);

                string zipFileName = $"{Path.GetFileName(sourceDirectory)}.zip";
                string zipFilePath = Path.Combine(folderPathImagem, zipFileName);

                if (System.IO.File.Exists(zipFilePath))
                {
                    System.IO.File.Delete(zipFilePath);
                }

                ZipFile.CreateFromDirectory(sourceDirectory, zipFilePath);
                Console.WriteLine($"Compactação de {categoria} concluida com sucesso.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao compactar. {ex.Message}");
                throw;
            }
        }

        public void CompactarPastaCategorias(string cnpjEmpresa = "02295529000105")
        {
            var variavel = BuscarVariavelAmbiente();

            if (string.IsNullOrEmpty(variavel))
                throw new Exception("Variavel de ambiente não encontrada");

            string folderPathBase = Path.Combine(variavel, "Arquivos", cnpjEmpresa);
 

            CompactarPasta( "Produto", folderPathBase);
            CompactarPasta( "Pessoa", folderPathBase);
            CompactarPasta( "Empresa", folderPathBase);
        }

        //public void CompactarPastaImagensCompleta()
        //{
        //    string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Arquivos", "02295529000105");
        //    string folderPathImagens = Path.Combine(folderPath, "Imagens");
            
        //    if (!Directory.Exists(folderPathImagens))
        //    {
        //        throw new Exception("A pasta especificada não existe.");
        //    }

        //    try
        //    {
        //        string zipFileName = $"{Path.GetFileName(folderPathImagens)}.zip";
        //        string zipFilePath = Path.Combine(folderPath, zipFileName);

        //        if (System.IO.File.Exists(zipFilePath))
        //        {
        //            System.IO.File.Delete(zipFilePath);
        //        }

        //        var sourceDirectory = Path.GetFullPath(folderPath);

        //        ZipFile.CreateFromDirectory(sourceDirectory, zipFilePath);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Erro ao compactar. {ex.Message}");

        //        throw;
        //    }
        //}
    }
}
