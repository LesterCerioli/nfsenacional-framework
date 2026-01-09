using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ns_nfsenacional_csharp;

namespace exemplos
{
    internal class Program
    {
        //Exemplo de uso do método emitirNFSeSincrono
        static async Task Main(string[] args)
        {
            try
            {
                string jsonPath = @"C:\\Projetos-INTeETC\\projetos andriel\\ns-nfsenacional-csharp\\exemplos\\nfse.json";
                string jsonData = File.ReadAllText(jsonPath);

                var retorno = NSNFSeNacional.emitirNFSeSincrono(jsonData, "json", "13278005000122", "XP", "2", @"C:\Projetos-INTeETC\projetos andriel\ns-nfsenacional-csharp\exemplos\notasEmitidas\", true);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Erro: {ex.Message}");
			}
		}
    }
}
