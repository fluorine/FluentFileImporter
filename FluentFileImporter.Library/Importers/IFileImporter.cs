using System.Collections.Generic;

namespace FileImporter
{
    /// <summary>
    /// File importer to generate entities of type <typeparamref name="E"/>.
    /// </summary>
    /// <typeparam name="E"></typeparam>
    public interface IFileImporter<out E> where E : new()
    {
        IEnumerable<E> GenerateEntitiesFromFile(string filePath);
    }
}