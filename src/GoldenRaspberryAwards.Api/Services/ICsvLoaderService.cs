namespace GoldenRaspberryAwards.Api.Services;

public interface ICsvLoaderService
{
    /// <summary>
    /// Carrega o CSV e insere apenas vencedores no banco, se a tabela estiver vazia.
    /// </summary>
    Task LoadIfEmptyAsync(string csvPath);
}
